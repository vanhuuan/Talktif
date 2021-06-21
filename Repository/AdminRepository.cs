using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Talktif.Models;

namespace Talktif.Repository
{
    public interface IAdminRepo
    {
        HttpResponseMessage Statistic(string token);
        HttpResponseMessage GetAllUser(long From,long To, string token);
        HttpResponseMessage GetAllReport(long From, long To, string token);
        HttpResponseMessage UpdateReport(UpdateReportRequest request,string token);
        HttpResponseMessage UpdateUser(UpdateUserRequest request, string token);
        HttpResponseMessage DeleteUser(int id, string token);
        HttpResponseMessage DeleteNonReferenceChatRoom(string token);
        HttpResponseMessage CreateNewAdmin(SignUpRequest request, string token);
    }
    public class AdminRepo : IAdminRepo
    {
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Admin/";
        public HttpResponseMessage Statistic(string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var statistics = client.GetAsync("Count");
                statistics.Wait();
                return statistics.Result;
            }
        }
        public HttpResponseMessage GetAllUser(long From,long To, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var getAllUser = client.GetAsync("GetAllUser/" + From + "/" + To + "/UserId");
                getAllUser.Wait();
                return getAllUser.Result;
            }
        }
        public HttpResponseMessage GetAllReport(long From, long To, string token)
        {
            using(var client = new HttpClient())
            {  
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var getAllReport = client.GetAsync("GetAllReport/" + From + "/" + To + "/ReportId");
                getAllReport.Wait();
                return getAllReport.Result;
            }
        }
        public HttpResponseMessage UpdateReport(UpdateReportRequest request,string token)
        {
            using(var client = new HttpClient())
            { 
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var upDateReport = client.PutAsJsonAsync("UpdateReport",request);
                upDateReport.Wait();
                return upDateReport.Result;
            }
        }
        public HttpResponseMessage UpdateUser(UpdateUserRequest request, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var upDateUser = client.PutAsJsonAsync("UpdateUser",request);
                upDateUser.Wait();
                return upDateUser.Result;
            }
        }
        public HttpResponseMessage DeleteUser(int id, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var deleteUser = client.DeleteAsync("DeleteUser/" + id);
                deleteUser.Wait();
                return deleteUser.Result;
            }
        }
        public HttpResponseMessage DeleteNonReferenceChatRoom(string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var deleteNonReferenceChatRoom = client.DeleteAsync("DeleteNonReferenceChatRoom");
                deleteNonReferenceChatRoom.Wait();
                return deleteNonReferenceChatRoom.Result;
            }
        }
        public HttpResponseMessage CreateNewAdmin(SignUpRequest request, string token)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var createNewAdmin = client.PostAsJsonAsync("CreateNewAdmin",request);
                createNewAdmin.Wait();
                return createNewAdmin.Result;
            }
        }
    }
}