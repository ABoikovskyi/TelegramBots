using System;

namespace DataLayer.Models.DTO
{
	public class IdrinkMessage
	{
		public DateTime DateCondition { get; set; }
		public bool IsDrank { get; set; }
		public string Body { get; set; }
	}
}