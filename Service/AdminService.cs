using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;

namespace Talktif.Service
{
    public interface IAdminService
    {
        Task<Statistic> GetStatisticData(HttpRequest Request, HttpResponse Response);
        Task<List<user>> GetAllUser(HttpRequest Request, HttpResponse Response,String Filter,String Search);
        Task<User_Infor> GetUserInfo(HttpRequest Request, HttpResponse Response, int ID_User);
        Task<bool> UpdateUser(HttpRequest Request, HttpResponse Response, UpdateUserRequest updateRequest);
        Task<bool> DeleteUser(HttpRequest Request, HttpResponse Response, int ID);
        Task<List<Report_Infor>> GetAllReport(HttpRequest Request, HttpResponse Response,String Filter,String Search);
        Task<Report_Infor> GetReportInfo(HttpRequest Request, HttpResponse Response, int ID_Report);
        Task<bool> UpdateReport(HttpRequest Request, HttpResponse Response, UpdateReportRequest updateRequest);
        Task<long> GetNumberofUser(HttpRequest Request, HttpResponse Response);
        Task<long> GetNumberofReport(HttpRequest Request, HttpResponse Response);
        Task<string> CreateNewAdmin(HttpRequest Request, HttpResponse Response, SignUpRequest request);
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
        public async Task<Statistic> GetStatisticData(HttpRequest Request, HttpResponse Response)
        {
            try
            {
                Statistic statistic = new Statistic();
                Cookie_Data cookie_Data = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.Statistic(cookie_Data.token);
                string statisticsResult = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Statistic>(statisticsResult);
                }
                else
                {
                    await _userService.RefreshToken(Response, cookie_Data);
                    cookie_Data = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.Statistic(cookie_Data.token);
                    statisticsResult = result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<Statistic>(statisticsResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Statistic();
            }
        }
        public async Task<List<user>> GetAllUser(HttpRequest Request, HttpResponse Response,String Filter,String Search)
        {
            try
            {
                long top = await GetNumberofUser(Request,Response);
                List<user> users = new List<user>();
                Cookie_Data cookie_Data = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.GetAllUser(top, cookie_Data.token,Filter,Search);
                string a = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    users = JsonConvert.DeserializeObject<List<user>>(a);
                    for (int i = 0; i < users.Count; i++)
                    {
                        users[i].cityID = await _userService.GetNameCity(Int32.Parse(users[i].cityID));
                    }
                }
                else
                {
                    Console.WriteLine(result.StatusCode);
                    await _userService.RefreshToken(Response, cookie_Data);
                    cookie_Data = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.GetAllUser(top, cookie_Data.token,Filter,Search);
                    a = result.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<user>>(a);
                    for (int i = 0; i < users.Count; i++)
                    {
                        users[i].cityID = await _userService.GetNameCity(Int32.Parse(users[i].cityID));
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<user>();
            }
        }
        public async Task<User_Infor> GetUserInfo(HttpRequest Request, HttpResponse Response, int ID_User)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.GetUserInfo(ID_User, cookie.token);
                string a = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<User_Infor>(a);
                }
                else
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.GetUserInfo(ID_User, cookie.token);
                    a = result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<User_Infor>(a);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new User_Infor();
            }
        }
        public async Task<bool> UpdateUser(HttpRequest Request, HttpResponse Response, UpdateUserRequest updateRequest)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.UpdateUser(updateRequest, cookie.token);
                if (result.IsSuccessStatusCode) return true;
                else if ((result.StatusCode.ToString() == "Unauthorized"))
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.UpdateUser(updateRequest, cookie.token);
                    return true;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public async Task<bool> DeleteUser(HttpRequest Request, HttpResponse Response, int ID)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.DeleteUser(ID, cookie.token);
                if (result.IsSuccessStatusCode) return true;
                else if ((result.StatusCode.ToString() == "Unauthorized"))
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.DeleteUser(ID, cookie.token);
                    return true;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public async Task<List<Report_Infor>> GetAllReport(HttpRequest Request, HttpResponse Response,String Filter,String Search)
        {
            try
            {
                long top = await GetNumberofReport(Request,Response);
                List<Report_Infor> reports = new List<Report_Infor>();
                Cookie_Data cookie_Data = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.GetAllReport(top, cookie_Data.token,Filter,Search);
                string a = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    reports = JsonConvert.DeserializeObject<List<Report_Infor>>(a);
                }
                else
                {
                    await _userService.RefreshToken(Response, cookie_Data);
                    cookie_Data = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.GetAllReport(top, cookie_Data.token,Filter,Search);
                    a = result.Content.ReadAsStringAsync().Result;
                    reports = JsonConvert.DeserializeObject<List<Report_Infor>>(a);
                }
                return reports;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<Report_Infor>();
            }
        }
        public async Task<Report_Infor> GetReportInfo(HttpRequest Request, HttpResponse Response, int ID_Report)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.GetReportInfo(ID_Report, cookie.token);
                string a = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Report_Infor>(a);
                }
                else
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.GetReportInfo(ID_Report, cookie.token);
                    a = result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<Report_Infor>(a);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Report_Infor();
            }
        }
        public async Task<bool> UpdateReport(HttpRequest Request, HttpResponse Response, UpdateReportRequest updateRequest)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.UpdateReport(updateRequest, cookie.token);
                if (result.IsSuccessStatusCode) return true;
                else if ((result.StatusCode.ToString() == "Unauthorized"))
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.UpdateReport(updateRequest, cookie.token);
                    return true;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public async Task<long> GetNumberofUser(HttpRequest Request, HttpResponse Response)
        {
            try
            {
                Statistic a = await GetStatisticData(Request, Response);
                return a.numOfUser;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public async Task<long> GetNumberofReport(HttpRequest Request, HttpResponse Response)
        {
            try
            {
                Statistic a = await GetStatisticData(Request, Response);
                return a.numOfReport;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public async Task<string> CreateNewAdmin(HttpRequest Request, HttpResponse Response, SignUpRequest request)
        {
            try
            {
                Cookie_Data cookie = _userService.ReadUserCookie(Request);
                var result = await _adminRepo.CreateNewAdmin(request, cookie.token);
                string message = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode) return null;
                else if ((result.StatusCode.ToString() == "Unauthorized"))
                {
                    await _userService.RefreshToken(Response, cookie);
                    cookie = _userService.ReadUserCookie(Request);
                    result = await _adminRepo.CreateNewAdmin(request, cookie.token);
                    message = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode) return null;
                    else return message;
                }
                else return message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "An error has occur !";
            }
        }
    }
}