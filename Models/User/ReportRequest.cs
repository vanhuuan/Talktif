namespace Talktif.Models
{
    public class ReportRequest
    {
        public int Reporter { get; set; }
        public int Suspect { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}