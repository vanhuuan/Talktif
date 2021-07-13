using System;

namespace Talktif.Models
{
    public class ChatBox
    {
        public int ID_Room { get; set; }
        public int ID_User { get; set; }
        public int ID_Partner { get; set; }
        public string Name_PartNer { get; set; }
        public int Sender { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastTime { get; set; }
    }
}