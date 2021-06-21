using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;
using Microsoft.AspNetCore.Http;

namespace Talktif.Service
{
    public interface IUserService
    {
        void CreateCookie(HttpResponse Response, Cookie_Data data);
        Cookie_Data ReadCookie(HttpRequest Request);
        bool IsAdmin(HttpRequest Request);
        void RemoveCookie(HttpResponse Response);
        HttpResponseMessage Sign_Up(SignUpRequest sr);
        HttpResponseMessage Sign_In(LoginRequest lr);
        HttpResponseMessage ResetPass(string email);
        User_Infor Get_User_Infor(HttpRequest Request, HttpResponse Response);
        void RefreshToken(HttpResponse Response, Cookie_Data cookie);
        List<City> GetCity();
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

        public void CreateCookie(HttpResponse Response, Cookie_Data data)
        {
            _cookieService.CreateCookie(Response, data);
        }
        public Cookie_Data ReadCookie(HttpRequest Request)
        {
            return _cookieService.ReadCookie(Request);
        }
        public bool IsAdmin(HttpRequest Request)
        {
            return _cookieService.IsAdmin(Request);
        }
        public void RemoveCookie(HttpResponse Response)
        {
            _cookieService.RemoveCookie(Response);
        }
        public HttpResponseMessage Sign_Up(SignUpRequest sr)
        {
            return _userRepo.Sign_Up(sr);
        }
        public HttpResponseMessage Sign_In(LoginRequest lr)
        {
            return _userRepo.Sign_In(lr);
        }
        public HttpResponseMessage ResetPass(string email)
        {
            return _userRepo.ResetPass(new ResetPassRequest() { Email = email });
        }
        public User_Infor Get_User_Infor(HttpRequest Request, HttpResponse Response)
        {
            var cookie = _cookieService.ReadCookie(Request);
            if (cookie == null) return null;
            var result = _userRepo.GetUserByID(cookie.id, cookie.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                User_Infor userInfo = JsonConvert.DeserializeObject<User_Infor>(a);
                userInfo.token = cookie.token;
                return userInfo;
            }
            else
            {
                RefreshToken(Response, cookie);
                cookie = _cookieService.ReadCookie(Request);
                result = _userRepo.GetUserByID(cookie.id, cookie.token);
                a = result.Content.ReadAsStringAsync().Result;
                User_Infor userInfo = JsonConvert.DeserializeObject<User_Infor>(a);
                userInfo.token = cookie.token;
                return userInfo;
            }
        }
        public void RefreshToken(HttpResponse Response, Cookie_Data cookie)
        {
            RefreshTokenRequest r = new RefreshTokenRequest() { Email = cookie.email };
            var refreshToken = _userRepo.RefreshToken(r, cookie.token);
            var result = refreshToken.Content.ReadAsStringAsync().Result;
            Token t = JsonConvert.DeserializeObject<Token>(result);
            cookie.token = t.token;
            _cookieService.UpdateCookie(Response, cookie);
        }
        public List<City> GetCity()
        {
            List<City> cities = new List<City>();
            var result = _userRepo.GetAllCityCountry(1);
            string Result = result.Content.ReadAsStringAsync().Result;
            cities = JsonConvert.DeserializeObject<List<City>>(Result);
            return cities;
        }
    }
}