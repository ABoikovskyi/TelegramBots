using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Festival
{
    [Table("UserSubscription")]
    public class UserSubscription
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public int ArtistId { get; set; }
		public virtual Artist Artist { get; set; }
	}
}