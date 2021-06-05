namespace Talktif.Models
{
    public class user
    {
        public int id { get; set;}
        public string name { get; set;}
        public string email { get; set;}
        public bool gender { get; set;}
        public bool isAdmin { get; set; }
        public bool isActive { get; set; }
        public string hobbies { get; set; }
        public string cityID { get; set; }
    }
}