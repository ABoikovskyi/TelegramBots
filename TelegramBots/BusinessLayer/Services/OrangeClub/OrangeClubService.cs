using System;
using System.Linq;
using DataLayer.Context;

namespace BusinessLayer.Services.OrangeClub
{
	public class OrangeClubService
	{
		private readonly OrangeClubDbContext _repository;

		public OrangeClubService(OrangeClubDbContext repository)
		{
			_repository = repository;
		}

		public string GetPromoCode()
		{
			var code = _repository.PromoCodes.FirstOrDefault(c => !c.InUse);
			if (code == null)
			{
				return "Извините, Вы не успели";
			}

			code.IssueData = DateTime.Now;
			code.InUse = true;
			_repository.Update(code);
			_repository.SaveChanges();

			return code.Code;
		}
	}
}