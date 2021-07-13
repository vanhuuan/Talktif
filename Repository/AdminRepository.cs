using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Talktif.Models;

namespace Talktif.Repository
{
    public interface IAdminRepo
    {
        Task<HttpResponseMessage> Statistic(string token);
        Task<HttpResponseMessage> GetAllUser(long From, long To, string token);
        Task<HttpResponseMessage> GetAllReport(long From, long To, string token);
        Task<HttpResponseMessage> UpdateReport(UpdateReportRequest request, string token);
        Task<HttpResponseMessage> UpdateUser(UpdateUserRequest request, string token);
        Task<HttpResponseMessage> DeleteUser(int id, string token);
        Task<HttpResponseMessage> DeleteNonReferenceChatRoom(string token);
        Task<HttpResponseMessage> CreateNewAdmin(SignUpRequest request, string token);
        Task<HttpResponseMessage> GetUserInfo(int id, string token);
        Task<HttpResponseMessage> GetReportInfo(int id, string token);
    }
    public class AdminRepo : IAdminRepo
    {
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Admin/";
        public async Task<HttpResponseMessage> Statistic(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("Count");
            }
        }
        public async Task<HttpResponseMessage> GetAllUser(long From, long To, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("GetAllUser/" + From + "/" + To + "/UserId");
            }
        }
        public async Task<HttpResponseMessage> GetAllReport(long From, long To, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("GetAllReport/" + From + "/" + To + "/ReportId");
            }
        }
        public async Task<HttpResponseMessage> UpdateReport(UpdateReportRequest request, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PutAsJsonAsync("UpdateReport", request);
            }
        }
        public async Task<HttpResponseMessage> UpdateUser(UpdateUserRequest request, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PutAsJsonAsync("UpdateUser", request);
            }
        }
        public async Task<HttpResponseMessage> DeleteUser(int id, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.DeleteAsync("DeleteUser/" + id);
            }
        }
        public async Task<HttpResponseMessage> DeleteNonReferenceChatRoom(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.DeleteAsync("DeleteNonReferenceChatRoom");
            }
        }
        public async Task<HttpResponseMessage> CreateNewAdmin(SignUpRequest request, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PostAsJsonAsync("CreateNewAdmin", request);
            }
        }
        public async Task<HttpResponseMessage> GetUserInfo(int id, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("GetUserInfo/" + id);
            }
        }
        public async Task<HttpResponseMessage> GetReportInfo(int id, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("GetReportInfo/" + id);
            }
        }
    }
}