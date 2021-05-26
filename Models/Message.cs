using System;
namespace Talktif.Models
{
    public class Message
    {
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}