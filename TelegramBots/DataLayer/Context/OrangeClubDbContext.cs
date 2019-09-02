using DataLayer.Models.OrangeClub;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class OrangeClubDbContext : DbContext
    {
        public OrangeClubDbContext(DbContextOptions<OrangeClubDbContext> options) : base(options)
        {
        }

        public virtual DbSet<PromoCode> PromoCodes { get; set; }
	}
}