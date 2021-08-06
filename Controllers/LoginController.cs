using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Talktif.Models;
using Newtonsoft.Json;
using Talktif.Service;
using System.Threading.Tasks;

namespace Talktif.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private IUserService _userService;
        private ICookieService _cookieService;

        public LoginController(ILogger<LoginController> logger, IUserService userService, ICookieService cookieService)
        {
            _logger = logger;
            _userService = userService;
            _cookieService = cookieService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _cookieService.RemoveCookie(Response, "email");
            _userService.RemoveUserCookie(Response);
            ViewBag.Cities = await _userService.GetCity();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Sign_In(IFormCollection form)
        {
            if (String.IsNullOrEmpty(form["Email"].ToString()) || String.IsNullOrEmpty(form["Password"].ToString()))
            {
                ViewBag.Message = "An error had occur !";
                ViewBag.Cities = await _userService.GetCity();
                return View("Index");
            }
            LoginRequest lr = new LoginRequest() { Email = form["Email"].ToString(), Password = form["Password"].ToString(), Device = (System.Net.Dns.GetHostName()).ToString() };
            var loginResult =await _userService.Sign_In(lr);
            string a = loginResult.Content.ReadAsStringAsync().Result;
            if (loginResult.IsSuccessStatusCode)
            {
                User_Infor user = JsonConvert.DeserializeObject<User_Infor>(a);
                Cookie_Data cookie_Data = new Cookie_Data() { id = user.id, IsAdmin = user.isAdmin, email = user.email, token = user.token };
                _userService.CreateUserCookie(Response, cookie_Data);
                return RedirectToAction("Index", "Home");
            }
            MessageRequest m = new MessageRequest() { Message = a };
            ViewBag.Message = a;
            ViewBag.Cities = await _userService.GetCity();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Sign_Up(IFormCollection form)
        {
            if (String.IsNullOrEmpty(form["Name"].ToString()) ||
                String.IsNullOrEmpty(form["Email"].ToString()) ||
                String.IsNullOrEmpty(form["Password"].ToString()) ||
                String.IsNullOrEmpty(form["Gender"].ToString()) ||
                String.IsNullOrEmpty(form["format"].ToString())
            )
            {
                ViewBag.Message = "An error had occur !";
                ViewBag.Cities = await _userService.GetCity();
                return View("Index");
            };

            string hobbies = form["Sport"].ToString()
                + (form["Sport"].ToString() != "" ? "," : "") + form["Study"].ToString()
                + (form["Study"].ToString() != "" ? "," : "") + form["Movie"].ToString()
                + (form["Movie"].ToString() != "" ? "," : "") + form["Game"].ToString()
                + (form["Game"].ToString() != "" ? "," : "") + form["Music"].ToString()
                + (form["Music"].ToString() != "" ? "," : "") + form["Reading"].ToString()
                + (form["Reading"].ToString() != "" ? "," : "") + form["Shopping"].ToString()
                + (form["Shopping"].ToString() != "" ? "," : "") + form["Travel"].ToString();

            SignUpRequest sr = new SignUpRequest()
            {
                Name = form["Name"].ToString(),
                Email = form["Email"].ToString(),
                Password = form["Password"].ToString(),
                Gender = (form["Gender"].ToString() == "Male") ? true : false,
                CityId = Convert.ToInt32(form["format"].ToString()),
                Hobbies = hobbies,
                Device = System.Environment.MachineName,
            };
            var signUpResult =await _userService.Sign_Up(sr);
            string a = signUpResult.Content.ReadAsStringAsync().Result;
            if (signUpResult.IsSuccessStatusCode)
            {
                User_Infor user = JsonConvert.DeserializeObject<User_Infor>(a);
                Cookie_Data cookie_Data = new Cookie_Data() { id = user.id, IsAdmin = user.isAdmin, email = user.email, token = user.token };
                _userService.CreateUserCookie(Response, cookie_Data);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = a;
            ViewBag.Cities = await _userService.GetCity();
            return View("Index");
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPass(IFormCollection form)
        {
            var resetPassResult =await _userService.CheckEmail(form["Email"].ToString());
            string a = resetPassResult.Content.ReadAsStringAsync().Result;
            Console.WriteLine(a);
            if (resetPassResult.IsSuccessStatusCode)
            {
                _cookieService.CreateCookie(Response, form["Email"].ToString(), "email");
                ViewBag.Email = form["Email"].ToString();
                return View("ResetPasswordEmail");
            }
            ViewBag.Message = a;
            return View("ForgotPass");
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordEmail(IFormCollection form)
        {
            string email = _cookieService.ReadCookie(Request, "email");
            string pass = form["pass"].ToString();
            if (String.IsNullOrEmpty(pass))
            {
                ViewBag.Message = "password is requied !";
                ViewBag.Email = email;
                return View("ResetPasswordEmail");
            }
            _cookieService.RemoveCookie(Response, "email");
            await _userService.ResetPass(Response, email, pass);
            return RedirectToAction("Index", "Home");
        }
    }
}