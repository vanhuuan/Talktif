using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Repository;

namespace Talktif.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        public IActionResult Home()
        {
            if(UserRepo.Instance.data.isAdmin != true) return NotFound();
            return View(UserRepo.Instance.data);
        }
        public IActionResult Users()
        {
            List<user> users = new List<user>();
            var result = AdminRepo.Instance.GetAllUser(8,0);
            if(result.IsSuccessStatusCode)
            {
                string a = result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<user>>(a);
                for(int i = 0; i < users.Count; i++)
                {
                    users[i].cityID = AdminRepo.Instance.GetNameCity(Int32.Parse(users[i].cityID));
                }
            }
            else
            {
                RefreshTokenRequest r = new RefreshTokenRequest(){Email = UserRepo.Instance.data.email};
                var refreshToken = UserRepo.Instance.RefreshToken(r);
                string a = refreshToken.Content.ReadAsStringAsync().Result;
                Console.WriteLine(a);
            }
            return View(users);
        }
        public IActionResult ReportUser()
        {
            return View();
        }
        public IActionResult Setting()
        {
            return View();
        }
    }
}