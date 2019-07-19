using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Idrink
{
	[Table("Log")]
	public class Log
	{
		public int Id { get; set; }
		public long ChatId { get; set; }
		public DateTime LogDate { get; set; }
		public string Message { get; set; }
		public string StackTrace { get; set; }
	}
}