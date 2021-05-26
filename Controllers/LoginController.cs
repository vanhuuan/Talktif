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

namespace Program.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Sign_In(IFormCollection form)
        {
            LoginRequest lr = new LoginRequest(){Email = form["Email"].ToString(),Password = form["Password"].ToString(),Device = (System.Environment.MachineName).ToString()};
            var loginResult = Repo.Instance.Sign_In(lr);
            Console.WriteLine(loginResult);
            if(loginResult.IsSuccessStatusCode){
                string a = loginResult.Content.ReadAsStringAsync().Result;
                Repo.Instance.data = JsonConvert.DeserializeObject<User_Infor>(a);
                
                //Repo.Instance.ShowInformation();
                
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Sign_Up(IFormCollection form)
        {
            SignUpRequest sr = new SignUpRequest(){
                Name = form["Name"].ToString(),
                Email = form["Email"].ToString(),
                Password = form["Password"].ToString(),
                Gender = true,
                CityId = 49,
                Hobbies = "hook up",
                Device = System.Environment.MachineName,
            };
            var signUpResult = Repo.Instance.Sign_Up(sr);
            if(signUpResult.IsSuccessStatusCode)
            {
                string a = signUpResult.Content.ReadAsStringAsync().Result;
                Repo.Instance.data = JsonConvert.DeserializeObject<User_Infor>(a);
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Login");
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
    }
}