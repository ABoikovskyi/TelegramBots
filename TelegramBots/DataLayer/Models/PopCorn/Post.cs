using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Models.Enums;

namespace DataLayer.Models.PopCorn
{
	public class Post
	{
		public int Id { get; set; }
		public int? ConcertId { get; set; }
		public Concert Concert { get; set; }
		public string Title { get; set; }
		public string Desription { get; set; }
		public string Link { get; set; }
		public DateTime Date { get; set; }
        public bool IsCommonPost { get; set; }
        public PostStatus Status { get; set; }
		public DateTime? PublishDate { get; set; }
		public DateTime? ScheduleDate { get; set; }

		[NotMapped]
		public virtual string StatusText => Status == PostStatus.Published
			? $" - Опубликован {PublishDate:dd-MM-yy HH:mm}"
			: Status == PostStatus.Scheduled
				? $" - Запланирован на {ScheduleDate:dd-MM-yy HH:mm}"
				: "";
	}
}