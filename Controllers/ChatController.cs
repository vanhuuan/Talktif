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
using Microsoft.AspNetCore.Http;

namespace Talktif.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private IChatService _chatService;
        private IChatRepo _chatRepo;

        public ChatController(ILogger<HomeController> logger, IUserService userService, IChatService chatService, IChatRepo chatRepo)
        {
            _logger = logger;
            _userService = userService;
            _chatService = chatService;
            _chatRepo = chatRepo;
        }

        public async Task<IActionResult> Index()
        {
            User_Infor usr = await _userService.Get_User_Infor(Request, Response);
            return View(usr);
        }
        // public async Task<IActionResult> Friends_Beta(int? id)
        // {
        //     // Get user info and pre-setup
        //     User_Infor usr = await _userService.Get_User_Infor(Request, Response);
        //     if (usr == null)
        //     {
        //         return RedirectToAction("Index", "Login");
        //     };

        //     FriendsViewModel vm = new FriendsViewModel
        //     {
        //         UserID = usr.id,
        //         UserToken = usr.token,
        //         RoomID = id != null ? (int)id : 0
        //     };

        //     // Fetch all chat rooms
        //     vm.RoomList = await _chatService.FetchAllChatRoom(Request, Response);

        //     // Fetch all messages
        //     if (vm.RoomID > 0)
        //     {
        //         vm.Messages = await _chatService.FetchMessage(Request, Response, vm.RoomID, 20);
        //     }
        //     return View(vm);
        // }
        // public async Task<IActionResult> Chat_Beta()
        // {
        //     User_Infor usr = await _userService.Get_User_Infor(Request, Response);
        //     return View(usr);
        // }

        public async Task<IActionResult> Friends(int? id)
        {
            // Get user info and pre-setup
            User_Infor usr = await _userService.Get_User_Infor(Request, Response);
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
            List<ChatBox> list = await _chatService.GetListChatBox(Request,Response,usr.id);
            ViewBag.ListChatBox = list;

            vm.RoomList = await _chatService.FetchAllChatRoom(Request, Response);

            // Fetch all messages
            if (vm.RoomID > 0)
            {
                vm.Messages = await _chatService.FetchMessage(Request, Response, vm.RoomID, 20);
            }
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Delete Chat Room
        public async Task<IActionResult> Unfriend(int uid, int roomid, string token)
        {
            await _chatRepo.DeleteChatRoom(uid, roomid, token);
            return RedirectToAction("Friends");
        }
    }
}
