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
        public async Task<IActionResult> Users(string PageNum)
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();

            long first = 8, last = 0;
            long NumberofUser = await _adminService.GetNumberofUser(Request, Response);

            if (String.IsNullOrEmpty(PageNum) == false)
            {
                last = first * (Int64.Parse(PageNum) - 1);
                first = (first * (Int64.Parse(PageNum)) > NumberofUser) ? NumberofUser : first * (Int64.Parse(PageNum));
                if (first < last) return NotFound();
            }
            first = (NumberofUser < first) ? NumberofUser : first;
            List<user> users = new List<user>();
            users = await _adminService.GetAllUser(Request, Response, first, last);

            ViewBag.Users = users;
            ViewBag.NumberofUser = NumberofUser;
            ViewBag.Cities = await _userService.GetCity();
            return View();
        }
        [HttpPost,ActionName("Users")]
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
            string a = await _adminService.CreateNewAdmin(Request,Response,data);
            if(String.IsNullOrEmpty(a))  return RedirectToAction("Users");
            else
            {
                ViewBag.Message = a;
                long numOfUser = await _adminService.GetNumberofUser(Request, Response);
                ViewBag.Users =await _adminService.GetAllUser(Request, Response, (numOfUser > 8) ? 8 : numOfUser, 0);
                ViewBag.NumberofUser = numOfUser;
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
            }
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
            var result =await _adminService.DeleteUser(Request,Response,ID);
            if(result == false) Console.WriteLine("An error has occur !");
            return RedirectToAction("Users");
        }
        public async Task<IActionResult> ReportUser(string PageNum)
        {
            if (_userService.IsAdmin(Request) != true) return NotFound();

            long first = 8, last = 0;
            long NumberofReport = await _adminService.GetNumberofReport(Request, Response);

            if (String.IsNullOrEmpty(PageNum) == false)
            {
                last = first * (Int64.Parse(PageNum) - 1);
                first = (first * (Int64.Parse(PageNum)) > NumberofReport) ? NumberofReport : first * (Int64.Parse(PageNum));
                if (first < last) return NotFound();
            }
            first = (NumberofReport > first) ? first : NumberofReport;
            List<Report_Infor> reports = new List<Report_Infor>();
            reports = await _adminService.GetAllReport(Request, Response, first, last);

            ViewBag.Reports = reports;
            ViewBag.NumberofReport = NumberofReport;
            return View();
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
            bool gender = (form["Gender"].ToString() == "Male") ? true : false;
            int CityId = Convert.ToInt32(form["format"].ToString());

            if (newpass != confirmpass)
            {
                ViewBag.message = "Confirm new password does not match";

                return View("AdminSetting");
            }
            else if (oldpass == "")
            {
                ViewBag.message = "The password field is requied";
                return View("AdminSetting");
            }
            else if (newpass == "")
            {
                newpass = oldpass;
            }
            string message = await _userService.UpdateUserInfor(Request, Response, name, email, newpass, oldpass, CityId, gender);
            if (message == null) return RedirectToAction("Home");
            else
            {
                ViewBag.message = message;
                return View("Setting");
            }
        }
    }
}