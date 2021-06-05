using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Talktif.Models;
using Talktif.Data;
using Talktif.Repository;

namespace Talktif.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserFavRepository _repository;

        public HomeController(ILogger<HomeController> logger, IUserFavRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if(UserRepo.Instance.data == null)
            {
                return RedirectToAction("Index","Login");
            }
            if(UserRepo.Instance.data.isAdmin == false) return RedirectToAction("Home","User");
            else return RedirectToAction("Home","Admin");
        }

        // [HttpPost]
        // public IActionResult Index(int userID, int toID = -1)
        // {
        //     ViewModel vm = new ViewModel(userID, _repository.FetchRoomID(userID, toID), _repository.FetchUserFavs(userID));
        //     return View(vm);
        // }

        // public IActionResult Privacy()
        // {
        //     return View();
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
