using System.Collections.Generic;
namespace Talktif.Models
{
    public class ViewModel {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public List<UserFav> UserFavs { get; set; }
        public ViewModel(int user, int room,List<UserFav> lu){
            this.UserID = user;
            this.RoomID = room;
            this.UserFavs = lu;
        }
    }
}