using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.OrangeClub
{
	[Table("PromoCodes")]
	public class PromoCode
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public bool InUse { get; set; }
		public DateTime? IssueData { get; set; }
	}
}