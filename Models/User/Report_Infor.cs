using System;

namespace Talktif.Models
{
    public class Report_Infor
    {
        public int id { get; set; }
        public int reporter { get; set; }
        public int suspect { get; set; }
        public string reason { get; set; }
        public string note { get; set; }
        public bool status { get; set; }
        //public DateTime createAt { get; set; }
    }
}