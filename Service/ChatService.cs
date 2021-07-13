using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;

namespace Talktif.Service
{
    public interface IChatService
    {
        Task<List<Room_Infor>> FetchAllChatRoom(HttpRequest Request, HttpResponse Response);
        Task<List<Message>> FetchMessage(HttpRequest Request, HttpResponse Response, int ID_Room, int TopMessage);
        Task<ChatRoom_Info> GetChatRoomInfo(HttpRequest Request, HttpResponse Response, int ID_Room);
        Task AddMessage(HttpRequest Request, HttpResponse Response, string message, int IDChatRoom);
        Task<List<ChatBox>> GetListChatBox(HttpRequest Request, HttpResponse Response, int ID_User);
    }
    public class ChatService : IChatService
    {
        private IChatRepo _chatRepo;
        private IUserService _userService;
        public ChatService(IChatRepo chatRepo, IUserService userService)
        {
            _chatRepo = chatRepo;
            _userService = userService;
        }
        public async Task<List<Room_Infor>> FetchAllChatRoom(HttpRequest Request, HttpResponse Response)
        {
            var cookie = _userService.ReadUserCookie(Request);
            var result = await _chatRepo.FetchAllChatRoom(cookie.id, cookie.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<Room_Infor>>(a);
            }
            else
            {
                await _userService.RefreshToken(Response, cookie);
                cookie = _userService.ReadUserCookie(Request);
                result = await _chatRepo.FetchAllChatRoom(cookie.id, cookie.token);
                a = result.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<Room_Infor>>(a);
            }
        }
        public async Task<List<Message>> FetchMessage(HttpRequest Request, HttpResponse Response, int ID_Room, int TopMessage)
        {
            var cookie = _userService.ReadUserCookie(Request);
            var result = await _chatRepo.FetchMessage(cookie.id, ID_Room, TopMessage, cookie.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<Message>>(a);
            }
            else
            {
                await _userService.RefreshToken(Response, cookie);
                cookie = _userService.ReadUserCookie(Request);
                result = await _chatRepo.FetchMessage(cookie.id, ID_Room, TopMessage, cookie.token);
                a = result.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<Message>>(a);
            }
        }
        public async Task<ChatRoom_Info> GetChatRoomInfo(HttpRequest Request, HttpResponse Response, int ID_Room)
        {
            var cookie = _userService.ReadUserCookie(Request);
            var result = await _chatRepo.GetChatRoomInfo(ID_Room, cookie.id, cookie.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ChatRoom_Info>(a);
            }
            else
            {
                await _userService.RefreshToken(Response, cookie);
                cookie = _userService.ReadUserCookie(Request);
                result = await _chatRepo.GetChatRoomInfo(ID_Room, cookie.id, cookie.token);
                a = result.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<ChatRoom_Info>(a);
            }
        }
        public async Task AddMessage(HttpRequest Request, HttpResponse Response, string message, int IDChatRoom)
        {
            var cookie = _userService.ReadUserCookie(Request);
            var result = await _chatRepo.AddMessage(message, cookie.id, IDChatRoom, cookie.token);
            if (!(result.IsSuccessStatusCode))
            {
                await _userService.RefreshToken(Response, cookie);
                cookie = _userService.ReadUserCookie(Request);
                result = await _chatRepo.AddMessage(message, cookie.id, IDChatRoom, cookie.token);
            }
        }
        public async Task<List<ChatBox>> GetListChatBox(HttpRequest Request, HttpResponse Response, int Id_User)
        {
            List<ChatBox> list = new List<ChatBox>();
            List<Room_Infor> room_Infors = await FetchAllChatRoom(Request, Response);
            foreach (var room in room_Infors)
            {
                ChatRoom_Info infor = await GetChatRoomInfo(Request, Response, room.id);
                List<Message> messages = await FetchMessage(Request, Response, room.id, 1);

                int Id_Partner, Sender = 0;
                string NamePartNer, Message = "";
                DateTime date = new DateTime(0);
                if (infor.user1Id == Id_User)
                {
                    Id_Partner = infor.user2Id;
                    NamePartNer = infor.nickName2;
                }
                else
                {
                    Id_Partner = infor.user1Id;
                    NamePartNer = infor.nickName1;
                }
                foreach (var mess in messages)
                {
                    Sender = mess.sender;
                    Message = mess.content;
                    date = mess.sentAt;
                }

                list.Add(new ChatBox()
                {
                    ID_User = Id_User,
                    ID_Room = room.id,
                    ID_Partner = Id_Partner,
                    Name_PartNer = NamePartNer,
                    Sender = Sender,
                    LastMessage = Message,
                    LastTime = date,
                }
                );
            }
            return list;
        }
    }
}