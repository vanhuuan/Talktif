using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Service;

namespace Talktif.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private IUserService _userService;
        private IAdminService _adminService;
        private ICookieService _cookieService;

        public AdminController(
            ILogger<AdminController> logger,
            IUserService userService,
            IAdminService adminService,
            ICookieService cookieService)
        {
            _logger = logger;
            _userService = userService;
            _adminService = adminService;
            _cookieService = cookieService;
        }
        public async Task<IActionResult> Home()
        {
            _cookieService.RemoveCookie(Response, "id");
            if (_userService.IsAdmin(Request) != true) return NotFound();
            ViewBag.Statistic = await _adminService.GetStatisticData(Request, Response);
            return View();
        }
        public async Task<IActionResult> Users(String filter="ID",String search="null")
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();
            List<user> us = new List<user>();
            us = await _adminService.GetAllUser(Request, Response,filter,search);
            List<user> users = new List<user>();
            for(int i = us.Count -1;i>=0;i--)
            {
                users.Add(us[i]);
            }
            ViewBag.Users = users;
            ViewBag.Cities = await _userService.GetCity();
            return View();
        }
        [HttpPost, ActionName("SearchUser")]
        public async Task<IActionResult> SearchUsers(IFormCollection form)
        {
            List<user> us = new List<user>();
            var fil = form["filter"].ToString();
            var sch = form["search"].ToString();
            if(sch.Equals("")) sch="null";
            await _adminService.GetStatisticData(null,null);
            return RedirectToAction("Users",new {filter = fil, search = sch});
        }
        [HttpPost, ActionName("Users")]
        public async Task<IActionResult> CreateNewAdmin(IFormCollection form)
        {
            SignUpRequest data = new SignUpRequest()
            {
                Name = form["name"].ToString(),
                Email = form["email"].ToString(),
                Password = form["pass"].ToString(),
                Gender = (form["Gender"].ToString() == "Male"),
                CityId = Convert.ToInt32(form["format"].ToString()),
                Device = System.Environment.MachineName
            };
            string a = await _adminService.CreateNewAdmin(Request, Response, data);
            if (String.IsNullOrEmpty(a)) return RedirectToAction("Users");
            else
            {
                ViewBag.Message = a;
                ViewBag.Users = await _adminService.GetAllUser(Request, Response,"ID","null");
                ViewBag.Cities = await _userService.GetCity();
                return View();
            }
        }
        public async Task<IActionResult> UpdateUser(int ID)
        {
            User_Infor user = new User_Infor();
            user = await _adminService.GetUserInfo(Request, Response, ID);

            _cookieService.CreateCookie(Response, ID.ToString(), "id");

            ViewBag.Infor = user;
            ViewBag.Cities = await _userService.GetCity();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(IFormCollection form)
        {
            int ID = Convert.ToInt32(_cookieService.ReadCookie(Request, "id"));
            var user = await _adminService.GetUserInfo(Request, Response, ID);
            _cookieService.RemoveCookie(Response, "id");
            if (String.IsNullOrEmpty(form["name"].ToString()))
            {
                ViewBag.Message = "The field name is requied !";
                return await UpdateUser(user.id);
            };
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Id = user.id,
                Name = form["name"].ToString(),
                Email = user.email,
                Gender = (form["Gender"].ToString() == "Male"),
                CityId = Convert.ToInt32(form["format"].ToString()),
                IsActive = (form["Active"].ToString() == "True"),
                createdAt = DateTime.Now,
            };
            Console.WriteLine(JsonConvert.SerializeObject(updateRequest));
            if (await _adminService.UpdateUser(Request, Response, updateRequest))
            {
                return RedirectToAction("Users");
            }
            else
            {
                ViewBag.Message = "An errol has occur";
                return await UpdateUser(user.id);
            }
        }
        public async Task<IActionResult> DeleteUser(int ID)
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();
            var result = await _adminService.DeleteUser(Request, Response, ID);
            if (result == false) Console.WriteLine("An error has occur !");
            return RedirectToAction("Users");
        }
        public async Task<IActionResult> ReportUser(String filter="ID",String search="null")
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();

            List<Report_Infor> rep = new List<Report_Infor>();
            rep = await _adminService.GetAllReport(Request, Response,filter,search);
            List<Report_Infor> reports = new List<Report_Infor>();
            for(int i = rep.Count -1;i>=0;i--)
            {
                reports.Add(rep[i]);
            }
            ViewBag.Reports = reports;
            return View();
        }
        [HttpPost, ActionName("SearchReport")]
        public async Task<IActionResult> SearchReport(IFormCollection form)
        {
            List<user> us = new List<user>();
            var fil = form["filter"].ToString();
            var sch = form["search"].ToString();
            if(sch.Equals("")) sch="null";
            await _adminService.GetStatisticData(null,null);
            return RedirectToAction("ReportUser",new {filter = fil, search = sch});
        }
        public async Task<IActionResult> UpdateReport(int ID)
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();
            Report_Infor report = await _adminService.GetReportInfo(Request, Response, ID);

            _cookieService.CreateCookie(Response, ID.ToString(), "id");

            ViewBag.Infor = report;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateReport(IFormCollection form)
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();
            int id = Convert.ToInt32(_cookieService.ReadCookie(Request, "id"));
            _cookieService.RemoveCookie(Response, "id");
            UpdateReportRequest data = new UpdateReportRequest()
            {
                Id = id,
                Note = String.IsNullOrEmpty(form["notes"].ToString()) ? "" : form["notes"].ToString(),
                Status = (form["Status"].ToString() == "True")
            };
            if (await _adminService.UpdateReport(Request, Response, data))
            {
                return RedirectToAction("Reportuser");
            }
            else
            {
                ViewBag.Message = "An errol has occur";
                return await UpdateReport(id);
            }
        }
        public async Task<IActionResult> Setting()
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();
            User_Infor user = await _userService.Get_User_Infor(Request, Response);
            ViewBag.Data = user;
            ViewBag.Cities = await _userService.GetCity();
            return View("AdminSetting");
        }
        [HttpPost]
        public async Task<IActionResult> Setting(IFormCollection form)
        {
            User_Infor user = await _userService.Get_User_Infor(Request, Response);
            ViewBag.Data = user;
            ViewBag.Cities = await _userService.GetCity();

            string oldpass = form["pass"].ToString();
            string newpass = form["newpass"].ToString();
            string confirmpass = form["confirmpassword"].ToString();
            string name = (String.IsNullOrEmpty(form["name"].ToString())) ? user.name : form["name"].ToString();
            string email = (String.IsNullOrEmpty(form["email"].ToString())) ? user.email : form["email"].ToString();
            string hobbies = form["Sport"].ToString()
                + (form["Sport"].ToString() != "" ? "," : "") + form["Study"].ToString()
                + (form["Study"].ToString() != "" ? "," : "") + form["Movie"].ToString()
                + (form["Movie"].ToString() != "" ? "," : "") + form["Game"].ToString()
                + (form["Game"].ToString() != "" ? "," : "") + form["Music"].ToString()
                + (form["Music"].ToString() != "" ? "," : "") + form["Reading"].ToString()
                + (form["Reading"].ToString() != "" ? "," : "") + form["Shopping"].ToString()
                + (form["Shopping"].ToString() != "" ? "," : "") + form["Travel"].ToString();
            bool gender = (form["Gender"].ToString() == "Male") ? true : false;
            int CityId = Convert.ToInt32(form["format"].ToString());

            if (newpass != confirmpass)
            {
                ViewBag.message = "Confirm new password does not match";

                ViewBag.Data = await _userService.Get_User_Infor(Request, Response);
                ViewBag.Cities = await _userService.GetCity();
                return View("AdminSetting");
            }
            else if (oldpass == "")
            {
                ViewBag.message = "The password field is requied";

                ViewBag.Data = await _userService.Get_User_Infor(Request, Response);
                ViewBag.Cities = await _userService.GetCity();
                return View("AdminSetting");
            }
            else if (newpass == "")
            {
                newpass = oldpass;
            }
            string message = await _userService.UpdateUserInfor(Request, Response, name, email, newpass, oldpass, CityId, gender, hobbies);
            if (message == null) return RedirectToAction("Home");
            else
            {
                ViewBag.message = message;
                ViewBag.Data = await _userService.Get_User_Infor(Request, Response);
                ViewBag.Cities = await _userService.GetCity();
                return View("AdminSetting");
            }
        }
    }
}