using System.Net.Http;
using System.Collections.Generic;
using Talktif.Models;
using System.Text.Json;
using System.IO;
using System.Text;
using System;

namespace Talktif.Data
{
    public interface IUserFavRepository
    {
        List<Room> FetchUserFavs(int userID);
        int FetchRoomID(int user, int destID);
    }
    public class UserFavRepository : IUserFavRepository
    {
        public List<Room> FetchUserFavs(int userID)
        {
            string jsonString = File.ReadAllText("Data/SampleUserFav.json", Encoding.UTF8);
            List<Room> data = JsonSerializer.Deserialize<List<Room>>(jsonString);
            foreach (Room i in data)
            {
                if (i.id == userID)
                {
                    data.Remove(i);
                    break;
                }
            }
            return data;
        }
        public int FetchRoomID(int user, int destinationUser)
        {
            string jsonString = File.ReadAllText("Data/SampleChatRoom.json", Encoding.UTF8);
            List<ChatRoom> data = JsonSerializer.Deserialize<List<ChatRoom>>(jsonString);
            List<ChatRoom> userRoom = new List<ChatRoom>();
            foreach (ChatRoom i in data)
            {
                if (i.Member == user)
                {
                    foreach (ChatRoom x in data)
                    {
                        if (x.Member == destinationUser && x.ID == i.ID) return x.ID;
                    }
                }
            }
            return -1;
        }
    }
}