using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Idrink
{
	[Table("Users")]
	public class User
	{
		public int Id { get; set; }
		public long ChatId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public bool IsActive { get; set; }
		public DateTime JoinDate { get; set; }
		public virtual List<DrinkHistory> DrinkHistory { get; set; }
	}
}