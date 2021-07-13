using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Talktif.Models;

namespace Talktif.Service
{
    public interface ICookieService
    {
        void CreateCookie(HttpResponse Response, string data,string key);
        void UpdateCookie(HttpResponse Response, string data,string key);
        string ReadCookie(HttpRequest Request,string key);
        void RemoveCookie(HttpResponse Response,string key);
    }
    public class CookieService : ICookieService
    {
        public void CreateCookie(HttpResponse Response, string data,string key)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(key, data, option);
        }
        public void UpdateCookie(HttpResponse Response, string data,string key)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(key, data, option);
        }
        public string ReadCookie(HttpRequest Request,string key)
        {
            string cookievalue = Request.Cookies[key];
            if (String.IsNullOrEmpty(cookievalue))
                return null;
            else
            {
                return cookievalue;
            }
        }
        public void RemoveCookie(HttpResponse Response,string key)
        {
            string value = string.Empty;
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Append(key, value, option);
        }
    }
}