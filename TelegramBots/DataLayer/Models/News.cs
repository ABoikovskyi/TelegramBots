using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
	[Table("News")]
	public class News
	{
		public int Id { get; set; }
		public int? ConcertId { get; set; }
		public Concert Concert { get; set; }
		public string Title { get; set; }
		public string Desription { get; set; }
		public string Link { get; set; }
		public DateTime Date { get; set; }
        public bool IsCommonPost { get; set; }
        public bool IsPublished { get; set; }
    }
}