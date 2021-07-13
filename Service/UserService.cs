using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Talktif.Service
{
    public interface IUserService
    {
        void CreateUserCookie(HttpResponse Response, Cookie_Data data);
        Cookie_Data ReadUserCookie(HttpRequest Request);
        bool IsAdmin(HttpRequest Request);
        void RemoveUserCookie(HttpResponse Response);
        Task<HttpResponseMessage> Sign_Up(SignUpRequest sr);
        Task<HttpResponseMessage> Sign_In(LoginRequest lr);
        Task<HttpResponseMessage> CheckEmail(string email);
        Task ResetPass(HttpResponse Response, string email, string pass);
        Task<User_Infor> Get_User_Infor(HttpRequest Request, HttpResponse Response);
        Task RefreshToken(HttpResponse Response, Cookie_Data cookie);
        Task<List<City>> GetCity();
        Task<string> GetNameCity(int cityID);
        Task<string> UpdateUserInfor(HttpRequest Request, HttpResponse Response, string name, string email, string pass, string oldpass, int cityID, bool gender);
    }
    public class UserService : IUserService
    {
        private IUserRepo _userRepo;
        private ICookieService _cookieService;
        public UserService(IUserRepo userRepo, ICookieService cookieService)
        {
            _userRepo = userRepo;
            _cookieService = cookieService;
        }

        public void CreateUserCookie(HttpResponse Response, Cookie_Data data)
        {
            string a = JsonConvert.SerializeObject(data);
            _cookieService.CreateCookie(Response, a, "user");
        }
        public Cookie_Data ReadUserCookie(HttpRequest Request)
        {
            try
            {
                return JsonConvert.DeserializeObject<Cookie_Data>(_cookieService.ReadCookie(Request, "user"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public bool IsAdmin(HttpRequest Request)
        {
            Cookie_Data a = ReadUserCookie(Request);
            return a.IsAdmin;
        }
        public void RemoveUserCookie(HttpResponse Response)
        {
            _cookieService.RemoveCookie(Response, "user");
        }
        public async Task<HttpResponseMessage> Sign_Up(SignUpRequest sr)
        {
            return await _userRepo.Sign_Up(sr);
        }
        public async Task<HttpResponseMessage> Sign_In(LoginRequest lr)
        {
            return await _userRepo.Sign_In(lr);
        }
        public async Task<HttpResponseMessage> CheckEmail(string email)
        {
            return await _userRepo.ResetPass(email);
        }
        public async Task ResetPass(HttpResponse Response, string email, string pass)
        {
            var result = await _userRepo.ResetPasswordEmail(new ResetPassEmailRequest() { Email = email, NewPass = pass });
            string a = result.Content.ReadAsStringAsync().Result;
            User_Infor user = JsonConvert.DeserializeObject<User_Infor>(a);
            Cookie_Data cookie = new Cookie_Data()
            {
                id = user.id,
                IsAdmin = user.isAdmin,
                email = user.email,
                token = user.token,
            };
            CreateUserCookie(Response, cookie);
        }
        public async Task<User_Infor> Get_User_Infor(HttpRequest Request, HttpResponse Response)
        {
            var cookie = ReadUserCookie(Request);
            if (cookie == null) return null;
            var result = await _userRepo.GetUserByID(cookie.id, cookie.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                User_Infor userInfo = JsonConvert.DeserializeObject<User_Infor>(a);
                userInfo.token = cookie.token;
                return userInfo;
            }
            else
            {
                await RefreshToken(Response, cookie);
                cookie = ReadUserCookie(Request);
                result = await _userRepo.GetUserByID(cookie.id, cookie.token);
                a = result.Content.ReadAsStringAsync().Result;
                User_Infor userInfo = JsonConvert.DeserializeObject<User_Infor>(a);
                userInfo.token = cookie.token;
                return userInfo;
            }
        }
        public async Task RefreshToken(HttpResponse Response, Cookie_Data cookie)
        {
            RefreshTokenRequest r = new RefreshTokenRequest() { Email = cookie.email };
            var refreshToken = await _userRepo.RefreshToken(r, cookie.token);
            var result = refreshToken.Content.ReadAsStringAsync().Result;
            Token t = JsonConvert.DeserializeObject<Token>(result);
            cookie.token = t.token;
            _cookieService.UpdateCookie(Response, JsonConvert.SerializeObject(cookie), "user");
        }
        public async Task<List<City>> GetCity()
        {
            List<City> cities = new List<City>();
            var result = await _userRepo.GetAllCityCountry(1);
            string Result = result.Content.ReadAsStringAsync().Result;
            cities = JsonConvert.DeserializeObject<List<City>>(Result);
            return cities;
        }
        public async Task<string> GetNameCity(int cityID)
        {
            foreach (var city in await GetCity())
            {
                if (cityID == city.id) return city.name;
            }
            return "";
        }
        public async Task<string> UpdateUserInfor(HttpRequest Request, HttpResponse Response, string name, string email, string pass, string oldpass, int cityID, bool gender)
        {
            var cookie = JsonConvert.DeserializeObject<Cookie_Data>(_cookieService.ReadCookie(Request, "user"));
            UpdateInfoRequest update = new UpdateInfoRequest()
            {
                Id = cookie.id,
                Name = name,
                Email = email,
                Password = pass,
                OldPassword = oldpass,
                CityId = cityID,
                Gender = gender,
            };
            var result = await _userRepo.UpdateUserInfor(update, cookie.token);
            string message = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                return "";
            }
            else if ((result.StatusCode.ToString() == "Unauthorized"))
            {
                await RefreshToken(Response, cookie);
                cookie = JsonConvert.DeserializeObject<Cookie_Data>(_cookieService.ReadCookie(Request, "user"));
                result = await _userRepo.UpdateUserInfor(update, cookie.token);
                message = result.Content.ReadAsStringAsync().Result;
                return message;
            }
            else return message;
        }
    }
}