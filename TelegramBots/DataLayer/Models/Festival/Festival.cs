using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("Festivals")]
    public class Festival
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte[] Map { get; set; }
        public string HowToGetTo { get; set; }
        public string CampingInfo { get; set; }
        public string Rules { get; set; }
        public string Contacts { get; set; }
        public virtual List<Stage> Stages { get; set; }
    }
}