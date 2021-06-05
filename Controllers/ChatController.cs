using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Data;
using Talktif.Repository;

namespace Talktif.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserFavRepository _repository;

        public ChatController(ILogger<HomeController> logger, IUserFavRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Index()
        {
            User_Infor usr = UserRepo.Instance.data;
            return View(usr);
        }
        public IActionResult Friends(int? id)
        {
            // Get user info and pre-setup
            User_Infor usr = UserRepo.Instance.data;
            if (usr == null)
            {
                return RedirectToAction("Index", "Login");
            };

            FriendsViewModel vm = new FriendsViewModel
            {
                UserID = usr.id,
                UserToken = usr.token,
                RoomID = id != null ? (int)id : 0
            };

            // Fetch all chat rooms
            var chatroomResult = ChatRepo.Instance.FetchAllChatRoom(usr.id);
            string crstring = chatroomResult.Content.ReadAsStringAsync().Result;
            if (chatroomResult.IsSuccessStatusCode)
            {
                vm.RoomList = JsonConvert.DeserializeObject<List<Room>>(crstring);
            }

            // Fetch all messages
            if (vm.RoomID > 0)
            {
                FetchMessageRequest req = new FetchMessageRequest
                {
                    RoomId = vm.RoomID,
                    Top = 20
                };
                var messagesResult = ChatRepo.Instance.FetchMessage(req);
                string mrstring = messagesResult.Content.ReadAsStringAsync().Result;
                if (messagesResult.IsSuccessStatusCode)
                {
                    vm.Messages = JsonConvert.DeserializeObject<List<Message>>(mrstring).OrderBy(m => m.sentAt).ToList();
                }
            }
            return View(vm);
        }

        // [HttpPost]
        // public IActionResult Index(int userID, int toID = -1)
        // {
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
