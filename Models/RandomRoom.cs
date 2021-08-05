using System.Collections.Generic;

namespace Talktif.Models
{
    public class RandomRoom
    {
        public string ID { get; set; }
        public WaitUser[] Members { get; set; }
        public void Display(){
            System.Console.WriteLine("RoomID: " + ID);
            foreach (WaitUser item in Members)
            {
                item.Display();
            }
        }
    }
}