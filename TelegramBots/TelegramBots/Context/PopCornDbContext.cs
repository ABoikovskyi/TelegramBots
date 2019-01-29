using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Context
{
    public class PopCornDbContext : DbContext
    {
        public PopCornDbContext(DbContextOptions<PopCornDbContext> options) : base(options)
        {
        }

        public virtual DbSet<MainInfo> MainInfo { get; set; }
        public virtual DbSet<Concert> Concerts { get; set; }
        public virtual DbSet<UserSubscription> UserSubscription { get; set; }
        public virtual DbSet<ConcertVisit> ConcertVisit { get; set; }
	    public virtual DbSet<User> Users { get; set; }
	    public virtual DbSet<News> News { get; set; }
	}
}