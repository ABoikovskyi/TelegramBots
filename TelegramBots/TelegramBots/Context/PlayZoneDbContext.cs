using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegramBots.Context
{
	public class PlayZoneDbContext : DbContext
	{
		public PlayZoneDbContext(DbContextOptions<PlayZoneDbContext> options) : base(options)
		{
		}

		public virtual DbSet<UserRequest> UserRequests { get; set; }
	}
}