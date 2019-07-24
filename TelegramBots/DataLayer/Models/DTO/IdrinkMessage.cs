using System;

namespace DataLayer.Models.DTO
{
	public class IdrinkMessage
	{
		public bool IsGlobal { get; set; }
		public DateTime DateCondition { get; set; }
		public bool IsDrank { get; set; }
		public string Body { get; set; }
		public int[] Users { get; set; }
	}
}