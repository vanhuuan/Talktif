using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Talktif.Models;
using Newtonsoft.Json;
using Talktif.Service;

namespace Talktif.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private IUserService _userService;

        public LoginController(ILogger<LoginController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Cities = _userService.GetCity();
            return View();
        }
        [HttpPost]
        public IActionResult Sign_In(IFormCollection form)
        {
            LoginRequest lr = new LoginRequest() { Email = form["Email"].ToString(), Password = form["Password"].ToString(), Device = (System.Environment.MachineName).ToString() };
            var loginResult = _userService.Sign_In(lr);
            string a = loginResult.Content.ReadAsStringAsync().Result;
            if (loginResult.IsSuccessStatusCode)
            {
                User_Infor user = JsonConvert.DeserializeObject<User_Infor>(a);
                _userService.CreateCookie(Response, new Cookie_Data() { id = user.id, IsAdmin = user.isAdmin, email = user.email, token = user.token });
                return RedirectToAction("Index", "Home");
            }
            MessageRequest m = new MessageRequest() { Message = a };
            ViewBag.Message = a;
            return View("Index");
        }
        [HttpPost]
        public IActionResult Sign_Up(IFormCollection form)
        {
            SignUpRequest sr = new SignUpRequest()
            {
                Name = form["Name"].ToString(),
                Email = form["Email"].ToString(),
                Password = form["Password"].ToString(),
                Gender = (form["Gender"].ToString() == "Male") ? true : false,
                CityId = Convert.ToInt32(form["format"].ToString()),
                Hobbies = "Travel",
                Device = System.Environment.MachineName,
            };
            var signUpResult = _userService.Sign_Up(sr);
            string a = signUpResult.Content.ReadAsStringAsync().Result;
            if (signUpResult.IsSuccessStatusCode)
            {
                //create cookie
                User_Infor user = JsonConvert.DeserializeObject<User_Infor>(a);
                _userService.CreateCookie(Response, new Cookie_Data() { id = user.id, IsAdmin = user.isAdmin, email = user.email, token = user.token });
                //create cookie
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = a;
            return View("Index");
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPass(IFormCollection form)
        {
            var resetPassResult = _userService.ResetPass(form["Email"].ToString());
            string a = resetPassResult.Content.ReadAsStringAsync().Result;
            Console.WriteLine(a);
            if (resetPassResult.IsSuccessStatusCode)
            {
                ViewBag.Email = "chuongthai885@gmail.com";
                return View("ResetPasswordEmail");
            }
            ViewBag.Message = a;
            return View("ForgotPass");
        }
        [HttpPost]
        public IActionResult ResetPasswordEmail(IFormCollection form)
        {
            return NotFound();
        }
    }
}