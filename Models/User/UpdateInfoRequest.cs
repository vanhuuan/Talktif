namespace Talktif.Models
{
    public class UpdateInfoRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public int CityId { get; set; }
        public bool Gender { get; set; }
        public string Hobbies { get; set; }
    }
}