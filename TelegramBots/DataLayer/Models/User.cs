using System.Collections.Generic;

namespace DataLayer.Models
{
	public class User
	{
		public int Id { get; set; }
		public string ChatId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public virtual List<ConcertVisit> ConcertVisits { get; set; }
        public virtual List<UserSubscription> Subscriptions { get; set; }
    }
}