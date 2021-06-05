namespace Talktif.Models
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public string Hobbies { get; set; }
    }
}