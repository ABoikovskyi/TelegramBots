using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Idrink
{
	[Table("Users")]
	public class User
	{
		public int Id { get; set; }
		public long ChatId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public virtual List<DrinkHistory> DrinkHistory { get; set; }
		/*public virtual List<Subscription> Subscribers { get; set; }
		public virtual List<Subscription> SubscribedsOn { get; set; }*/
	}
}