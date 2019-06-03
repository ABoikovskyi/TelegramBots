using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("Artists")]
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Itunes { get; set; }
        public virtual List<Schedule> ScheduleData { get; set; }
    }
}