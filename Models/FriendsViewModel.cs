using System.Collections.Generic;
namespace Talktif.Models
{
    public class FriendsViewModel {
        public int UserID { get; set; }
        public string UserToken { get; set; }
        public int RoomID { get; set; }
        public ChatRoom_Info RoomInfo { get; set; }
        public List<Room_Infor> RoomList { get; set; }
        public List<Message> Messages { get; set; }
        // public ViewModel(int user, int room,List<Room> roomlist){
        //     this.UserID = user;
        //     this.RoomID = room;
        //     this.RoomList = roomlist;
        // }
    }
}