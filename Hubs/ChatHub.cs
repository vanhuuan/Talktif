using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talktif.Models;
using Talktif.Repository;
using Talktif.Service;

namespace Talktif.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private void Debug()
        {
            // DEBUG
            System.Console.WriteLine("\nRoom");
            foreach (RandomRoom item in RoomManager.Instance.RoomList)
            {
                System.Console.WriteLine(item.ID);
                foreach (WaitUser usr in item.Members)
                {
                    System.Console.WriteLine(usr.ConnectionID);
                    System.Console.WriteLine(usr.FriendRequest);
                }
            }
            System.Console.WriteLine("Queue");
            foreach (WaitUser item in QueueManager.Instance.UserQueue)
            {
                System.Console.WriteLine(item.ConnectionID);
            }
        }
        public async Task SendMessage(string message)
        {
            RandomRoom room = RoomManager.Instance.GetRoom(Context.ConnectionId);
            if (room != null)
            {
                await Clients.Group(room.ID).ReceiveMessage(Context.ConnectionId, message);
            }
            Debug();
        }
        public async Task AddToQueue(int userID, string username, string filter)
        {
            RandomRoom room = QueueManager.Instance.Enqueue(new WaitUser
            {
                ConnectionID = Context.ConnectionId,
                UserID = userID,
                UserName = username,
                Filter = filter
            });

            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    await Groups.AddToGroupAsync(usr.ConnectionID, room.ID);
                    await Clients.Group(room.ID).BroadcastMessage($"Người dùng đã tham gia phòng chat.");
                }
            }

            // DEBUG
            Debug();
        }

        public async Task LeaveChat(int userID, string username)
        {
            if (RoomManager.Instance.GetRoom(Context.ConnectionId) != null)
            {
                // If user leaves while in a room
                RandomRoom room = RoomManager.Instance.RemoveRoom(new WaitUser
                {
                    ConnectionID = Context.ConnectionId,
                    UserID = userID,
                    UserName = username
                });
                if (room != null)
                {
                    foreach (WaitUser usr in room.Members)
                    {
                        await Clients.Group(room.ID).BroadcastMessage($"Người dùng đã rời khỏi phòng chat.");
                        await Groups.RemoveFromGroupAsync(usr.ConnectionID, room.ID);

                        // Join possible room after leave old room
                        RandomRoom newroom = RoomManager.Instance.GetRoom(usr.ConnectionID);
                        if (newroom != null)
                        {
                            foreach (WaitUser usr1 in newroom.Members)
                            {
                                await Groups.AddToGroupAsync(usr1.ConnectionID, newroom.ID);
                                await Clients.Group(newroom.ID).BroadcastMessage($"Người dùng đã tham gia phòng chat.");
                            }
                        }
                    }
                }
            }
            else
            {
                // If user leaves while in queue
                QueueManager.Instance.Dequeue(Context.ConnectionId);
            }

            // DEBUG
            Debug();
        }

        public async Task SkipChat(int userID, string username)
        {
            WaitUser user = new WaitUser
            {
                ConnectionID = Context.ConnectionId,
                UserID = userID,
                UserName = username
            };
            RandomRoom usrroom = RoomManager.Instance.GetRoom(user.ConnectionID);
            if (usrroom != null)
            {
                foreach (WaitUser usr in usrroom.Members)
                {
                    if (usr.ConnectionID != user.ConnectionID)
                    {
                        user.SkipID.Add(usr.ConnectionID);
                    }
                };

            }

            RandomRoom room = RoomManager.Instance.RemoveRoom(user);
            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    await Clients.Group(room.ID).BroadcastMessage($"Người dùng đã rời khỏi phòng chat.");
                    await Groups.RemoveFromGroupAsync(usr.ConnectionID, room.ID);

                    // Join possible room after leave old room
                    RandomRoom newroom = RoomManager.Instance.GetRoom(usr.ConnectionID);
                    if (newroom != null)
                    {
                        foreach (WaitUser usr1 in newroom.Members)
                        {
                            await Groups.AddToGroupAsync(usr1.ConnectionID, newroom.ID);
                            await Clients.Group(newroom.ID).BroadcastMessage($"Người dùng đã tham gia phòng chat.");
                        }
                    }
                }
            }

            // DEBUG
            Debug();
        }

        public async Task AddFriend(string token)
        {
            RandomRoom room = RoomManager.Instance.GetRoom(Context.ConnectionId);
            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    if (usr.UserID <= 0)
                    {
                        await Clients.Group(room.ID).BroadcastMessage($"Người dùng đề xuất kết bạn thất bại vì ít nhất 1 trong 2 người chưa đăng nhập!");
                        return;
                    }
                    if (usr.ConnectionID == Context.ConnectionId) usr.FriendRequest = true;
                }

                await Clients.Group(room.ID).BroadcastMessage($"Người dùng đã đề xuất kết bạn!");

                // Call API create chat room
                if (room.Members.Length >= 2 && room.Members[0].FriendRequest && room.Members[1].FriendRequest)
                {
                    IChatRepo chatRepo = new ChatRepo();
                    await chatRepo.CreateChatRoom(
                        room.Members[0].UserID,
                        room.Members[1].UserID,
                        room.Members[0].UserName,
                        room.Members[1].UserName,
                        token);
                }
            }

            Debug();
        }

        public async Task SaveFilter(int userID, string username, string filter)
        {
            await LeaveChat(userID, username);
            await AddToQueue(userID, username, filter);
        }

        // Friend Chat
        public async Task JoinFriendChat(string roomID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
            await Clients.Group(roomID).BroadcastMessage($"Người dùng đã tham gia phòng chat.");
        }
        public async Task LeaveFriendChat(string roomID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomID);
            await Clients.Group(roomID).BroadcastMessage($"Người dùng đã tham gia phòng chat.");
        }
        public async Task SendFriendMessage(string roomID, string message)
        {
            await Clients.Group(roomID).ReceiveMessage(Context.ConnectionId, message);
        }

        public async Task ReportUser(string userID, string reason, string note, string token)
        {
            RandomRoom room = RoomManager.Instance.GetRoom(Context.ConnectionId);
            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    if (usr.UserID <= 0)
                    {
                        await Clients.Group(room.ID).BroadcastMessage($"Báo cáo người dùng thất bại vì người dùng chưa đăng nhập!");
                        return;
                    }
                    if (usr.ConnectionID != Context.ConnectionId) {
                        UserRepo userRepo = new UserRepo();
                        ReportRequest report = new ReportRequest
                        {
                            Reporter = userID,
                            Suspect = usr.UserID.ToString(),
                            Reason = reason,
                            Note = note
                        };
                        await userRepo.Report(report, token);
                        await Clients.Group(room.ID).BroadcastMessage($"Đã gửi báo cáo thành công!");
                    };
                }
            }

            Debug();

        }
    }
}