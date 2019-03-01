using System.Collections.Generic;

namespace DataLayer.Models.DTO
{
	public class NBBarUserData
	{
		public string UserId { get; set; }
		public int? StepId { get; set; }
		public List<int> Ingredient { get; set; }
	}
}