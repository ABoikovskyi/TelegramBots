namespace DataLayer.Models
{
	public class UserSubscription
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public int ConcertId { get; set; }
		public virtual Concert Concert { get; set; }
	}
}