using System;

namespace Talktif.Models
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public int CityId { get; set; }
        public bool IsActive { get; set; }
        public DateTime createdAt { get; set; }
    }
}