using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Talktif.Models;
using Talktif.Repository;
using Microsoft.AspNetCore.Http;

namespace Talktif.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        public IActionResult Home()
        {
            //if(Repo.Instance.data.isAdmin == true) return NotFound();
            return View(Repo.Instance.data);
        }
        public IActionResult History()
        {
            return View(Repo.Instance.data);
        }
        [HttpPost]
        public IActionResult Home(IFormCollection formCollection)
        {
            Repo.Instance.data = null;
            return RedirectToAction("Login","Login");
        }
    }
}