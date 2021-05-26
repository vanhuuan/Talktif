using System;

namespace Talktif.Models
{
    public class SignUpRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set;}
        public bool Gender { get; set; }
        public int CityId  { get; set; }
        public string Hobbies { get; set; }
        public string Device { get; set; }
    }
}