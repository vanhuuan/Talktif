namespace Talktif.Models
{
    public class UserFav {
        public int ID { get; set; }
        public string Nickname { get; set; }
        public override string ToString()
        {
            return ID.ToString() + " " + Nickname;
        }
    }
}