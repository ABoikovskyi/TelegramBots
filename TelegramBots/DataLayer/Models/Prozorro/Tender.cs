using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Prozorro
{
	[Table("Tenders")]
	public class Tender
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Token { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		public double? Amount { get; set; }
		public string Currency { get; set; }

		public virtual List<Subscription> Subscribers { get; set; }
	}
}