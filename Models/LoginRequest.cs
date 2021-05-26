using System;

namespace Talktif.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Device { get; set; }
    }
}