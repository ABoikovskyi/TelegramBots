using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Prozorro
{
	[Table("Users")]
	public class User
	{
		public int Id { get; set; }
		public long ChatId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public DateTime JoinDate { get; set; }
		public bool IsRegistered { get; set; }
		public string Phone { get; set; }
		public DateTime? RegistrationDate { get; set; }

		public virtual List<Subscription> Subscriptions { get; set; }
	}
}