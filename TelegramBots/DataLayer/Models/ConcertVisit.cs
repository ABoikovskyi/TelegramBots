using System;

namespace DataLayer.Models
{
    public class ConcertVisit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
	    public User User { get; set; }
		public int ConcertId { get; set; }
        public virtual Concert Concert { get; set; }
        public DateTime VisitDate { get; set; }
    }
}