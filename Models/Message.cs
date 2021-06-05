using System;
namespace Talktif.Models
{
    public class Message
    {
        public int id { get; set; }
        public int sender { get; set; }
        public string content { get; set; }
        public DateTime sentAt { get; set; }
    }
}