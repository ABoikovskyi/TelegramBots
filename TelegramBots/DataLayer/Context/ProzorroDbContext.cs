using DataLayer.Models.Prozorro;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class ProzorroDbContext : DbContext
    {
        public ProzorroDbContext(DbContextOptions<ProzorroDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Tender> Tenders { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
	}
}