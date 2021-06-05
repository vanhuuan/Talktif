using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Talktif.Models;
using Talktif.Repository;

namespace Talktif.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(string message)
        {
            RandomRoom room = RoomManager.Instance.GetRoom(Context.ConnectionId);
            if (room != null)
            {
                await Clients.Group(room.ID).ReceiveMessage(Context.ConnectionId, message);
            }

            // DEBUG
            // System.Console.WriteLine("\nRoom");
            // foreach (RandomRoom item in RoomManager.Instance.RoomList)
            // {
            //     System.Console.WriteLine(item.ID);
            //     foreach (WaitUser usr in item.Members)
            //     {
            //         System.Console.WriteLine(usr.ConnectionID);
            //     }
            // }
            // System.Console.WriteLine("Queue");
            // foreach (WaitUser item in QueueManager.Instance.UserQueue)
            // {
            //     System.Console.WriteLine(item.ConnectionID);
            // }
        }
        public async Task AddToQueue(int userID, string username)
        {
            RandomRoom room = QueueManager.Instance.Enqueue(new WaitUser
            {
                ConnectionID = Context.ConnectionId,
                UserID = userID,
                UserName = username
            });

            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    await Groups.AddToGroupAsync(usr.ConnectionID, room.ID);
                    await Clients.Group(room.ID).BroadcastMessage($"Người dùng {usr.ConnectionID} đã tham gia phòng chat {room.ID}.");
                }
            }

            // DEBUG
            // System.Console.WriteLine("\nRoom");
            // foreach (RandomRoom item in RoomManager.Instance.RoomList)
            // {
            //     System.Console.WriteLine(item.ID);
            //     foreach (WaitUser usr in item.Members)
            //     {
            //         System.Console.WriteLine(usr.ConnectionID);
            //     }
            // }
            // System.Console.WriteLine("Queue");
            // foreach (WaitUser item in QueueManager.Instance.UserQueue)
            // {
            //     System.Console.WriteLine(item.ConnectionID);
            // }
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
                        await Clients.Group(room.ID).BroadcastMessage($"Người dùng {usr.ConnectionID} đã rời khỏi phòng chat {room.ID}.");
                        await Groups.RemoveFromGroupAsync(usr.ConnectionID, room.ID);

                        // Join possible room after leave old room
                        RandomRoom newroom = RoomManager.Instance.GetRoom(usr.ConnectionID);
                        if (newroom != null)
                        {
                            foreach (WaitUser usr1 in newroom.Members)
                            {
                                await Groups.AddToGroupAsync(usr1.ConnectionID, newroom.ID);
                                await Clients.Group(newroom.ID).BroadcastMessage($"Người dùng {usr1.ConnectionID} đã tham gia phòng chat {newroom.ID}.");
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
            // System.Console.WriteLine("\nRoom");
            // foreach (RandomRoom item in RoomManager.Instance.RoomList)
            // {
            //     System.Console.WriteLine(item.ID);
            //     foreach (WaitUser usr in item.Members)
            //     {
            //         System.Console.WriteLine(usr.ConnectionID);
            //         System.Console.WriteLine("Skip");
            //         foreach (string sid in usr.SkipID)
            //         {
            //             System.Console.WriteLine(sid);
            //         }
            //     }
            // }
            // System.Console.WriteLine("Queue");
            // foreach (WaitUser item in QueueManager.Instance.UserQueue)
            // {
            //     System.Console.WriteLine(item.ConnectionID);
            // }
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
                    await Clients.Group(room.ID).BroadcastMessage($"Người dùng {usr.ConnectionID} đã rời khỏi phòng chat {room.ID}.");
                    await Groups.RemoveFromGroupAsync(usr.ConnectionID, room.ID);

                    // Join possible room after leave old room
                    RandomRoom newroom = RoomManager.Instance.GetRoom(usr.ConnectionID);
                    if (newroom != null)
                    {
                        foreach (WaitUser usr1 in newroom.Members)
                        {
                            await Groups.AddToGroupAsync(usr1.ConnectionID, newroom.ID);
                            await Clients.Group(newroom.ID).BroadcastMessage($"Người dùng {usr1.ConnectionID} đã tham gia phòng chat {newroom.ID}.");
                        }
                    }
                }
            }

            // DEBUG
            // System.Console.WriteLine("\nRoom");
            // foreach (RandomRoom item in RoomManager.Instance.RoomList)
            // {
            //     System.Console.WriteLine(item.ID);
            //     foreach (WaitUser usr in item.Members)
            //     {
            //         System.Console.WriteLine(usr.ConnectionID);
            //     }
            // }
            // System.Console.WriteLine("Queue");
            // foreach (WaitUser item in QueueManager.Instance.UserQueue)
            // {
            //     System.Console.WriteLine(item.ConnectionID);
            // }
        }

        public async Task AddFriend()
        {
            RandomRoom room = RoomManager.Instance.GetRoom(Context.ConnectionId);
            if (room != null)
            {
                foreach (WaitUser usr in room.Members)
                {
                    if (usr.UserID <= 0) {
                        await Clients.Group(room.ID).BroadcastMessage($"Người dùng {Context.ConnectionId} đề xuất kết bạn thất bại vì ít nhất 1 trong 2 người chưa đăng nhập!");
                        return;
                    }
                    if (usr.ConnectionID == Context.ConnectionId) usr.FriendRequest = true;
                }

                await Clients.Group(room.ID).BroadcastMessage($"Người dùng {Context.ConnectionId} đã đề xuất kết bạn!");

                // Call API create chat room
                if (room.Members.Length >= 2 && room.Members[0].FriendRequest && room.Members[1].FriendRequest)
                {
                    ChatRepo.Instance.CreateChatRoom(new CreateChatRoomRequest
                    {
                        User1Id = room.Members[0].UserID,
                        User2Id = room.Members[1].UserID,
                        User1NickName = room.Members[0].UserName,
                        User2NickName = room.Members[1].UserName
                    });
                }
            }
        }

        // Friend Chat
        public async Task JoinFriendChat(string roomID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
            await Clients.Group(roomID).BroadcastMessage($"Người dùng {Context.ConnectionId} đã tham gia phòng chat {roomID}.");
        }
        public async Task LeaveFriendChat(string roomID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomID);
            await Clients.Group(roomID).BroadcastMessage($"Người dùng {Context.ConnectionId} đã tham gia phòng chat {roomID}.");
        }
        public async Task SendFriendMessage(string roomID, string message)
        {
            await Clients.Group(roomID).ReceiveMessage(Context.ConnectionId, message);
        }
    }
}