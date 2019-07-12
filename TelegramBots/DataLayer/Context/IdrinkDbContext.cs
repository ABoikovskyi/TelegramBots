using DataLayer.Models.Idrink;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class IdrinkDbContext : DbContext
    {
        public IdrinkDbContext(DbContextOptions<IdrinkDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
		public virtual DbSet<DrinkHistory> DrinkHistory { get; set; }
	}
}