using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Talktif.Models;

namespace Talktif.Repository
{
    public interface IChatRepo
    {
        HttpResponseMessage CreateChatRoom(CreateChatRoomRequest newchatroom, string token);
        HttpResponseMessage FetchAllChatRoom(int userid, string token);
        HttpResponseMessage FetchMessage(int UserID, int RoomID, int Top, string token);
        HttpResponseMessage GetChatRoomInfo(GetChatRoomInfoRequest room, string token);
        HttpResponseMessage AddMessage(AddMessageRequest mess, string token);
        HttpResponseMessage  DeleteChatRoom(DeleteChatRoomRequest d, string token);
    }
    public class ChatRepo : IChatRepo
    {
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Chat/";
        public HttpResponseMessage CreateChatRoom(CreateChatRoomRequest newchatroom, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var createchatroom = client.PostAsJsonAsync("CreateChatRoom",newchatroom);
                createchatroom.Wait();
                return createchatroom.Result;
            }
        }
        public HttpResponseMessage FetchAllChatRoom(int userid, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var fetchallchatroom = client.GetAsync("FetchAllChatRoom/" + userid);
                fetchallchatroom.Wait();
                return fetchallchatroom.Result;
            }
        }
        public HttpResponseMessage FetchMessage(int UserID, int RoomID, int Top, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var fetchMessage = client.GetAsync("FetchMessage/" + UserID + "/" + RoomID +"/" + Top);
                fetchMessage.Wait();
                return fetchMessage.Result;
            }
        }
        public HttpResponseMessage GetChatRoomInfo(GetChatRoomInfoRequest room, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var getChatRoomInfo = client.GetAsync("GetChatRoomInfo/" + room.Id + "/" + room.UserId);
                getChatRoomInfo.Wait();
                return getChatRoomInfo.Result;
            }
        }
        public HttpResponseMessage AddMessage(AddMessageRequest mess, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var addMessage = client.PostAsJsonAsync("AddMessage",mess);
                addMessage.Wait();
                return addMessage.Result;
            }
        }
        public HttpResponseMessage  DeleteChatRoom(DeleteChatRoomRequest d, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var deleteChatRoom = client.DeleteAsync("Delete/" + d.UserId + "/" + d.RoomId);
                deleteChatRoom.Wait();
                return deleteChatRoom.Result;
            }
        }
    } 
}