using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("Users")]
    public class User
	{
		public int Id { get; set; }
		public string ChatId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public virtual List<UserSubscription> Subscriptions { get; set; }
	}
}