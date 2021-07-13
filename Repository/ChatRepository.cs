using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Talktif.Models;

namespace Talktif.Repository
{
    public interface IChatRepo
    {
        Task<HttpResponseMessage> CreateChatRoom(int this_UserID, int that_UserID, string this_UserIDNickName, string that_UserIDNickName, string token);
        Task<HttpResponseMessage> FetchAllChatRoom(int userid, string token);
        Task<HttpResponseMessage> FetchMessage(int UserID, int RoomID, int Top, string token);
        Task<HttpResponseMessage> GetChatRoomInfo(int ID, int UserID, string token);
        Task<HttpResponseMessage> AddMessage(string message, int IDSender, int IDChatRoom, string token);
        Task<HttpResponseMessage> DeleteChatRoom(int UserID, int RoomID, string token);
    }
    public class ChatRepo : IChatRepo
    {
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Chat/";
        public async Task<HttpResponseMessage> CreateChatRoom(int this_UserID, int that_UserID, string this_UserIDNickName, string that_UserIDNickName, string token)
        {
            using (var client = new HttpClient())
            {
                CreateChatRoomRequest chatroom = new CreateChatRoomRequest()
                {
                    User1Id = this_UserID,
                    User2Id = that_UserID,
                    User1NickName = this_UserIDNickName,
                    User2NickName = that_UserIDNickName
                };
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PostAsJsonAsync("CreateChatRoom", chatroom);
            }
        }
        public async Task<HttpResponseMessage> FetchAllChatRoom(int userid, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("FetchAllChatRoom/" + userid);
            }
        }
        public async Task<HttpResponseMessage> FetchMessage(int UserID, int RoomID, int Top, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("FetchMessage/" + UserID + "/" + RoomID + "/" + Top);
            }
        }
        public async Task<HttpResponseMessage> GetChatRoomInfo(int ID, int UserID, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("GetChatRoomInfo/" + ID + "/" + UserID);
            }
        }
        public async Task<HttpResponseMessage> AddMessage(string message, int IDSender, int IDChatRoom, string token)
        {
            using (var client = new HttpClient())
            {
                AddMessageRequest mess = new AddMessageRequest()
                {
                    Message = message,
                    IdSender = IDSender,
                    IdChatRoom = IDChatRoom,
                };
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PostAsJsonAsync("AddMessage", mess);
            }
        }
        public async Task<HttpResponseMessage> DeleteChatRoom(int UserID, int RoomID, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.DeleteAsync("Delete/" + UserID + "/" + RoomID);
            }
        }
    }
}