using System;
using System.Collections.Generic;

namespace Talktif.Models
{
    public class WaitUser
    {
        public string ConnectionID { get; set; }
        public string Filter { get; set; }
        public List<string> SkipID { get; set; }
        public int UserID { get; set; }
        public String UserName { get; set; }
        public bool FriendRequest { get; set; }
        public WaitUser()
        {
            SkipID = new List<string>();
            UserID = 0;
            FriendRequest = false;
        }
        public void Display()
        {
            System.Console.WriteLine("ConnectionID: " + ConnectionID);
            for (int i = 0 ; i< SkipID.Count;i++)
            {
                System.Console.WriteLine("Skip: " + SkipID[i]);
            }
        }
    }
}