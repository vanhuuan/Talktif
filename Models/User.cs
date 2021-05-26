using System;

namespace Talktif.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool gender { get; set;}
        public string hobbies { get; set; }
        public bool isAdmin { get; set; }
        public bool isActive { get; set; }
        public DateTime createAt { get; set; }
    }
}