using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Talktif.Models;

namespace Talktif.Repository
{
    public interface IUserRepo
    {
        Task<HttpResponseMessage> Sign_Up(SignUpRequest Sr);
        Task<HttpResponseMessage> Sign_In(LoginRequest lr);
        Task<HttpResponseMessage> ResetPass(string email);
        Task<HttpResponseMessage> ResetPasswordEmail(ResetPassEmailRequest resetPassEmailRequest);
        Task<HttpResponseMessage> GetAllCountry();
        Task<HttpResponseMessage> GetAllCityCountry(int id);
        Task<HttpResponseMessage> GetUserByID(int ID, string token);
        Task<HttpResponseMessage> UpdateUserInfor(UpdateInfoRequest updateInfoRequest, string token);
        Task<HttpResponseMessage> InActiveUser(int id, string token);
        Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest, string token);
        Task<HttpResponseMessage> Report(ReportRequest request, string token);
    }
    public class UserRepo : IUserRepo
    {
        public RefreshToken rtoken { get; set; }
        private const string UriString = "https://talktifapi.azurewebsites.net/api/Users/";
        public async Task<HttpResponseMessage> Sign_Up(SignUpRequest Sr)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(UriString);
            var signUp = await client.PostAsJsonAsync("SignUp", Sr);
            Uri uri = new Uri(UriString);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            this.rtoken = new RefreshToken();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == "RefreshToken") rtoken.Refreshtoken = cookie.Value;
                if (cookie.Name == "RefreshTokenId") rtoken.RefreshTokenId = Convert.ToInt32(cookie.Value);
            }
            return signUp;
        }
        public async Task<HttpResponseMessage> Sign_In(LoginRequest lr)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(UriString);
            var login = await client.PostAsJsonAsync("SignIn", lr);
            Uri uri = new Uri(UriString);
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            this.rtoken = new RefreshToken();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == "RefreshToken") rtoken.Refreshtoken = cookie.Value;
                if (cookie.Name == "RefreshTokenId") rtoken.RefreshTokenId = Convert.ToInt32(cookie.Value);
            }
            return login;
        }
        public async Task<HttpResponseMessage> ResetPass(string email)
        {
            using (var client = new HttpClient())
            {
                ResetPassRequest resetPassRequest = new ResetPassRequest() { Email = email };
                client.BaseAddress = new Uri(UriString);
                return await client.PostAsync("ResetPass", new StringContent(JsonConvert.SerializeObject(resetPassRequest), Encoding.UTF8, "application/json"));
            }
        }
        public async Task<HttpResponseMessage> ResetPasswordEmail(ResetPassEmailRequest resetPassEmailRequest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                return await client.PostAsJsonAsync("ResetPasswordEmail", resetPassEmailRequest);
            }
        }
        public async Task<HttpResponseMessage> GetAllCountry()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                return await client.GetAsync("GetAllCountry");
            }
        }
        public async Task<HttpResponseMessage> GetAllCityCountry(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                return await client.GetAsync("GetAllCityCountry/" + id);
            }
        }
        public async Task<HttpResponseMessage> GetUserByID(int ID, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync(ID.ToString());
            }
        }
        public async Task<HttpResponseMessage> UpdateUserInfor(UpdateInfoRequest updateInfoRequest, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PutAsJsonAsync("UpdateInfo", updateInfoRequest);
            }
        }
        public async Task<HttpResponseMessage> InActiveUser(int id, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.GetAsync("InActiveUser/" + id);
            }
        }
        public async Task<HttpResponseMessage> RefreshToken(RefreshTokenRequest refreshTokenRequest, string token)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(UriString);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            cookies.Add(client.BaseAddress, new Cookie("RefreshToken", rtoken.Refreshtoken));
            cookies.Add(client.BaseAddress, new Cookie("RefreshTokenId", rtoken.RefreshTokenId.ToString()));
            return await client.PostAsJsonAsync("RefreshToken", refreshTokenRequest);
        }
        public async Task<HttpResponseMessage> Report(ReportRequest request, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UriString);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.PostAsJsonAsync("Report", request);
            }
        }
    }
}