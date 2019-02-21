using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Models.Enums;

namespace DataLayer.Models
{
	public class UserRequest
	{
		public int Id { get; set; }
		public string UserId { get; set; }

		[NotMapped]
		public RequestStep Step { get; set; }

		public string ContactFirstName { get; set; }
		public string ContactLastName { get; set; }
		public DateTime CreateDate { get; set; }
		public PlayZone? ZoneId { get; set; }
		public NumberOfPeople? NumberOfPeople { get; set; }
		public GameConsole? GameConsole { get; set; }
		public string Game { get; set; }
		public DateTime? RequestDate { get; set; }
		public string UserName { get; set; }
		public string UserPhone { get; set; }
		public RequestStatus Status { get; set; }
	}
}