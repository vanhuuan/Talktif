using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Talktif.Models;

namespace Talktif.Repository
{
    public class ChatRepo
    {
        private static ChatRepo _Instance;
        public static ChatRepo Instance {
            get 
            {
                if(_Instance == null)
                _Instance = new ChatRepo();
                return _Instance;
            }
            private set { }
        }
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Chat/";
        public HttpResponseMessage CreateChatRoom(CreateChatRoomRequest newchatroom)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var createchatroom = client.PostAsJsonAsync("CreateChatRoom",newchatroom);
                createchatroom.Wait();
                return createchatroom.Result;
            }
        }
        public HttpResponseMessage FetchAllChatRoom(int userid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var fetchallchatroom = client.GetAsync("FetchAllChatRoom/" + userid);
                fetchallchatroom.Wait();
                return fetchallchatroom.Result;
            }
        }
        public HttpResponseMessage FetchMessage(FetchMessageRequest request)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var fetchMessage = client.GetAsync("FetchMessage/" + request.RoomId +"/" + request.Top);
                fetchMessage.Wait();
                return fetchMessage.Result;
            }
        }
        public HttpResponseMessage GetChatRoomInfo(GetChatRoomInfoRequest room)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var getChatRoomInfo = client.GetAsync("GetChatRoomInfo/" + room.Id + "/" + room.UserId);
                getChatRoomInfo.Wait();
                return getChatRoomInfo.Result;
            }
        }
        public HttpResponseMessage AddMessage(AddMessageRequest mess)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var addMessage = client.PostAsJsonAsync("AddMessage",mess);
                addMessage.Wait();
                return addMessage.Result;
            }
        }
        public HttpResponseMessage  DeleteChatRoom(DeleteChatRoomRequest d)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",UserRepo.Instance.data.token);
                var deleteChatRoom = client.DeleteAsync("Delete/" + d.UserId + "/" + d.RoomId);
                deleteChatRoom.Wait();
                return deleteChatRoom.Result;
            }
        }
    } 
}