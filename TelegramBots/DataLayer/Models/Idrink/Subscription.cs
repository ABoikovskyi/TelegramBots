using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Idrink
{
	[Table("Subscriptions")]
	public class Subscription
	{
		public int Id { get; set; }

		[ForeignKey("Subscriber")]
		public int SubscriberId { get; set; }

		public virtual User Subscriber { get; set; }

		[ForeignKey("SubscribedOn")]
		public int SubscribedOnId { get; set; }

		public virtual User SubscribedOn { get; set; }
	}
}