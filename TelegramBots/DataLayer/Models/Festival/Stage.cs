using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("Stages")]
    public class Stage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FestivalId { get; set; }
        public virtual Festival Festival { get; set; }
        public virtual List<Schedule> ScheduleData { get; set; }
    }
}