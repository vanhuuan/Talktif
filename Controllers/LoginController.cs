using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Talktif.Models;
using Talktif.Repository;
using Newtonsoft.Json;

namespace Talktif.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index(MessageRequest m)
        {
            return View(m);
        }
        [HttpPost]
        public IActionResult Sign_In(IFormCollection form)
        {
            LoginRequest lr = new LoginRequest(){Email = form["Email"].ToString(),Password = form["Password"].ToString(),Device = (System.Environment.MachineName).ToString()};
            var loginResult = UserRepo.Instance.Sign_In(lr);
            //Console.WriteLine(loginResult);
            string a = loginResult.Content.ReadAsStringAsync().Result;
            if(loginResult.IsSuccessStatusCode){
                UserRepo.Instance.data = new User_Infor();
                UserRepo.Instance.data = JsonConvert.DeserializeObject<User_Infor>(a);
                return RedirectToAction("Index","Home");
            }
            MessageRequest m = new MessageRequest(){Message = a};
            return RedirectToAction("Login",m);
        }
        [HttpPost]
        public IActionResult Sign_Up(IFormCollection form)
        {
            SignUpRequest sr = new SignUpRequest(){
                Name = form["Name"].ToString(),
                Email = form["Email"].ToString(),
                Password = form["Password"].ToString(),
                Gender = true,
                CityId = 15,
                Hobbies = "",
                Device = System.Environment.MachineName,
            };
            var signUpResult = UserRepo.Instance.Sign_Up(sr);
            string a = signUpResult.Content.ReadAsStringAsync().Result;
            if(signUpResult.IsSuccessStatusCode)
            {
                UserRepo.Instance.data = JsonConvert.DeserializeObject<User_Infor>(a);
                return RedirectToAction("Index","Home");
            }
            MessageRequest m = new MessageRequest(){Message = a};
            return RedirectToAction("Login",m);
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPass(IFormCollection form)
        {
            ResetPassRequest rp = new ResetPassRequest(){Email = form["Email"].ToString()};
            var resetPassResult = UserRepo.Instance.ResetPass(rp);
            Console.WriteLine(resetPassResult);
            string a = resetPassResult.Content.ReadAsStringAsync().Result;
            if(resetPassResult.IsSuccessStatusCode)
            {
                //sent a to new page
                return RedirectToAction("ResetPasswordEmail");
            }//else sent to old page and the message 
            return NotFound();
        }
        public IActionResult ResetPasswordEmail()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPasswordEmail(IFormCollection form)
        {
            return NotFound();
        }
    }
}