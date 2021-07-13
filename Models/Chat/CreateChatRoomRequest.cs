using System;

namespace Talktif.Models
{
    public class CreateChatRoomRequest
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public string User1NickName { get; set; }
        public string User2NickName { get; set; }
    }
}