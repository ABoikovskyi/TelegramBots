using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    [Table("MainInfo")]
    public class MainInfo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string HelloText { get; set; }
        public string AboutText { get; set; }
        public string ContactsText { get; set; }
        public string TicketsText { get; set; }
        public string ConcertsText { get; set; }
        public string SalesText { get; set; }
    }
}
