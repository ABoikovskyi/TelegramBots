using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Enums
{
	public enum RequestStatus
	{
		[Display(Name="В обработке")]
		InProgress = 1,
		[Display(Name = "Подтверждена")]
		Approved,
		[Display(Name = "Отклонена")]
		Rejected
	}
}