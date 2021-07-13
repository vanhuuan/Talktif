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
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IUserService _userService;
        private IChatService _chatService;
        public UserController(ILogger<UserController> logger, IUserService userService, IChatService chatService)
        {
            _logger = logger;
            _userService = userService;
            _chatService = chatService;
        }
        public async Task<IActionResult> Home()
        {
            ViewBag.Cities = await _userService.GetCity();
            User_Infor user = await _userService.Get_User_Infor(Request, Response);
            ViewBag.nameUser = user.name;
            ViewBag.nameCity = await _userService.GetNameCity(user.cityId);
            List<ChatBox> list = await _chatService.GetListChatBox(Request,Response,user.id);
            ViewBag.ListChatBox = list;
            return View();
        }
        public IActionResult Logout()
        {
            _userService.RemoveUserCookie(Response);
            return RedirectToAction("Index", "Login");
        }
        public async Task<IActionResult> Index()
        {
            User_Infor user =await _userService.Get_User_Infor(Request, Response);
            ViewBag.Data = user;
            ViewBag.Cities =await _userService.GetCity();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            User_Infor user = await _userService.Get_User_Infor(Request, Response);
            ViewBag.Data = user;
            ViewBag.Cities =await _userService.GetCity();

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

                return View("Setting");
            }
            else if (oldpass == "")
            {
                ViewBag.message = "The password field is requied";
                return View("Setting");
            }
            else if (newpass == "")
            {
                newpass = oldpass;
            }
            string message =await _userService.UpdateUserInfor(Request, Response, name, email, newpass, oldpass, CityId, gender);
            if (message == null) return RedirectToAction("Home");
            else
            {
                ViewBag.message = message;
                return View("Setting");
            }
        }
    }
}