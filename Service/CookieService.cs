using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Talktif.Models;

namespace Talktif.Service
{
    public interface ICookieService
    {
        void CreateCookie(HttpResponse Response, Cookie_Data data);
        void UpdateCookie(HttpResponse Response, Cookie_Data cookie);
        Cookie_Data ReadCookie(HttpRequest Request);
        bool IsAdmin(HttpRequest Request);
        void RemoveCookie(HttpResponse Response);
    }
    public class CookieService : ICookieService
    {
        public void CreateCookie(HttpResponse Response, Cookie_Data data)
        {
            string key = "user";
            string value = JsonConvert.SerializeObject(data);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(key, value, option);
        }
        public void UpdateCookie(HttpResponse Response, Cookie_Data cookie)
        {
            string key = "user";
            string value = JsonConvert.SerializeObject(cookie);
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(key, value, option);
        }
        public Cookie_Data ReadCookie(HttpRequest Request)
        {
            string key = "user";
            string cookievalue = Request.Cookies[key];
            if (String.IsNullOrEmpty(cookievalue))
                return null;
            else
            {
                Cookie_Data a = new Cookie_Data();
                a = JsonConvert.DeserializeObject<Cookie_Data>(cookievalue);
                return a;
            }
        }
        public bool IsAdmin(HttpRequest Request)
        {
            string key = "user";
            string cookievalue = Request.Cookies[key];
            Cookie_Data a = JsonConvert.DeserializeObject<Cookie_Data>(cookievalue);
            return a.IsAdmin;
        }
        public void RemoveCookie(HttpResponse Response)
        {
            string key = "user";
            string value = string.Empty;
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Append(key, value, option);
        }
        private bool CheckCookie(HttpRequest Request)
        {
            string key = "user";
            string cookievalue = Request.Cookies[key];
            if (String.IsNullOrEmpty(cookievalue))
                return false;
            else return true;
        }
    }
}