using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Idrink
{
    [Table("Subscriptions")]
    public class Subscription
	{
		public int Id { get; set; }
		public int SubscriberId { get; set; }
		public virtual User Subscriber { get; set; }
		public int SubscribedOnId { get; set; }
		public virtual User SubscribedOn { get; set; }
	}
}