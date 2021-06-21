using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Talktif.Models;
using Talktif.Service;
using Talktif.Repository;

namespace Talktif.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private IChatRepo _chatRepo;

        public ChatController(ILogger<HomeController> logger, IUserService userService, IChatRepo chatRepo)
        {
            _logger = logger;
            _userService = userService;
            _chatRepo = chatRepo;
        }

        public IActionResult Index()
        {
            User_Infor usr = _userService.Get_User_Infor(Request, Response);
            return View(usr);
        }
        public IActionResult Friends(int? id)
        {
            // Get user info and pre-setup
            User_Infor usr = _userService.Get_User_Infor(Request, Response);
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
            var chatroomResult = _chatRepo.FetchAllChatRoom(usr.id, usr.token);
            string crstring = chatroomResult.Content.ReadAsStringAsync().Result;
            if (chatroomResult.IsSuccessStatusCode)
            {
                vm.RoomList = JsonConvert.DeserializeObject<List<Room>>(crstring);
            }

            // Fetch all messages
            if (vm.RoomID > 0)
            {
                var messagesResult = _chatRepo.FetchMessage(usr.id, vm.RoomID, 20, usr.token);
                string mrstring = messagesResult.Content.ReadAsStringAsync().Result;
                if (messagesResult.IsSuccessStatusCode)
                {
                    vm.Messages = JsonConvert.DeserializeObject<List<Message>>(mrstring).OrderBy(m => m.sentAt).ToList();
                }
            }
            return View(vm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
