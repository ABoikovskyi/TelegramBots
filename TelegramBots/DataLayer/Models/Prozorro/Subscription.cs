using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Prozorro
{
	[Table("Subscriptions")]
	public class Subscription
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public int TenderId { get; set; }
		public virtual Tender Tender { get; set; }
	}
}