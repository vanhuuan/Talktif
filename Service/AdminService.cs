using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;

namespace Talktif.Service
{
    public interface IAdminService
    {
        Statistic GetStatisticData(HttpRequest Request);
        List<user> GetAllUser(HttpRequest Request, HttpResponse Response, long first, long last);
        List<Report_Infor> GetAllReport(HttpRequest Request, HttpResponse Response, long first, long last);
        long GetNumberofUser(HttpRequest Request);
        long GetNumberofReport(HttpRequest Request);
        string GetNameCity(int cityID);
    }
    public class AdminService : IAdminService
    {
        private IUserService _userService;
        private IAdminRepo _adminRepo;
        public AdminService(
            IAdminRepo adminRepo,
            IUserService userService)
        {
            _userService = userService;
            _adminRepo = adminRepo;
        }
        public Statistic GetStatisticData(HttpRequest Request)
        {
            string token = (_userService.ReadCookie(Request)).token;
            var result = _adminRepo.Statistic(token);
            var statisticsResult = result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Statistic>(statisticsResult);
        }
        public List<user> GetAllUser(HttpRequest Request, HttpResponse Response, long first, long last)
        {
            List<user> users = new List<user>();
            Cookie_Data cookie_Data = _userService.ReadCookie(Request);
            var result = _adminRepo.GetAllUser(first, last, cookie_Data.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                users = JsonConvert.DeserializeObject<List<user>>(a);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].cityID = GetNameCity(Int32.Parse(users[i].cityID));
                }
            }
            else
            {
                _userService.RefreshToken(Response, cookie_Data);
                cookie_Data = _userService.ReadCookie(Request);
                result = _adminRepo.GetAllUser(first, last, cookie_Data.token);
                a = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<user>>(a);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].cityID = GetNameCity(Int32.Parse(users[i].cityID));
                }
            }
            return users;
        }
        public List<Report_Infor> GetAllReport(HttpRequest Request, HttpResponse Response, long first, long last)
        {
            List<Report_Infor> reports = new List<Report_Infor>();
            Cookie_Data cookie_Data = _userService.ReadCookie(Request);
            var result = _adminRepo.GetAllReport(first, last, cookie_Data.token);
            string a = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                reports = JsonConvert.DeserializeObject<List<Report_Infor>>(a);
            }
            else
            {
                Console.WriteLine("Toang");
                _userService.RefreshToken(Response, cookie_Data);
                cookie_Data = _userService.ReadCookie(Request);
                result = _adminRepo.GetAllReport(first, last, cookie_Data.token);
                a = result.Content.ReadAsStringAsync().Result;
                reports = JsonConvert.DeserializeObject<List<Report_Infor>>(a);
            }
            return reports;
        }
        public long GetNumberofUser(HttpRequest Request)
        {
            Statistic a = GetStatisticData(Request);
            return a.numOfUser;
        }
        public long GetNumberofReport(HttpRequest Request)
        {
            Statistic a = GetStatisticData(Request);
            return a.numOfReport;
        }
        public string GetNameCity(int cityID)
        {
            foreach (var city in _userService.GetCity())
            {
                if (cityID == city.id) return city.name;
            }
            return "";
        }
    }
}