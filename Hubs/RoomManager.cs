using System;
using System.Collections.Generic;
using Talktif.Models;

namespace Talktif.Hubs
{
    public class RoomManager
    {
        public List<RandomRoom> RoomList { get; set; }
        private RoomManager()
        {
            RoomList = new List<RandomRoom>();
        }
        private static RoomManager _Instance;
        public static RoomManager Instance
        {
            get
            {
                if (_Instance == null) _Instance = new RoomManager();
                return _Instance;
            }
            private set
            {
            }
        }
        public RandomRoom SearchRoom(string id)
        {
            foreach (RandomRoom r in RoomList)
            {
                if (r.ID == id) return r;
            }
            return null;
        }
        public RandomRoom GetRoom(string uid)
        {
            foreach (RandomRoom r in RoomList)
            {
                foreach (WaitUser usr in r.Members)
                {
                    if (usr.ConnectionID == uid) return r;
                }
            }
            return null;
        }
        public RandomRoom CreateRoom(WaitUser[] members)
        {
            string roomID;
            do
            {
                // create a random string of 8 characters
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                char[] stringChars = new char[8];
                Random random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                roomID = new String(stringChars);
            } while (SearchRoom(roomID) != null);

            RandomRoom room = new RandomRoom
            {
                ID = roomID,
                Members = members
            };
            RoomList.Add(room);
            return room;
        }
        public RandomRoom RemoveRoom(WaitUser user)
        {
            for (int i = 0; i < RoomList.Count; i++)
            {
                for (int j = 0; j < RoomList[i].Members.Length; j++)
                {
                    if (RoomList[i].Members[j].ConnectionID == user.ConnectionID)
                    {
                        RoomList[i].Members.SetValue(user, j);
                        RandomRoom room = RoomList[i];

                        RoomList.RemoveAt(i);

                        // Enqueue room memebers after removal
                        foreach (WaitUser usr1 in room.Members)
                        {
                            if (user.SkipID.Count > 0)
                            {
                                QueueManager.Instance.Enqueue(usr1);
                            }
                            else
                            {
                                if (usr1.ConnectionID != user.ConnectionID) QueueManager.Instance.Enqueue(usr1);
                            }
                        }

                        // return old room before removal
                        return room;
                    }
                }
            }
            return null;
        }
    }
}