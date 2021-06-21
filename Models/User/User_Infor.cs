using System;

namespace Talktif.Models
{
    public class User_Infor
    {
        public int id { get; set;}
        public string name { get; set;}
        public string email { get; set;}
        public bool gender { get; set;}
        public bool isAdmin { get; set; }
        public bool isActive { get; set; }
        public string hobbies { get; set; }
        public int cityId {get;set;}
        public string token { get; set; }
    }
}