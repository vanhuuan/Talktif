using System.Collections.Generic;

namespace Talktif.Models
{
    public class WaitUser
    {
        public string ConnectionID { get; set; }
        public string Filter { get; set; }
        public List<string> SkipID { get; set; }
        public User_Infor info { get; set; }
        public WaitUser()
        {
            SkipID = new List<string>();
        }
        public void Display()
        {
            System.Console.WriteLine("UserID: " + ConnectionID);
            for (int i = 0 ; i< SkipID.Count;i++)
            {
                System.Console.WriteLine("Skip: " + SkipID[i]);
            }
        }
    }
}