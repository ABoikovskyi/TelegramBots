using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("Schedule")]
    public class Schedule
    {
        public int Id { get; set; }
        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}