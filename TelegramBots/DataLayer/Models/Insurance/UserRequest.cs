using System.IO;
using DataLayer.Models.Enums;

namespace DataLayer.Models.Insurance
{
	public class UserRequest
	{
		public long UserId { get; set; }
		public InsuranceStep Step { get; set; }
		public string Text { get; set; }
		public Stream Photo { get; set; }
		public string PhotoName { get; set; }
	}
}