using DataLayer.Models.PlayZone;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
	public class PlayZoneDbContext : DbContext
	{
		public PlayZoneDbContext(DbContextOptions<PlayZoneDbContext> options) : base(options)
		{
		}

		public virtual DbSet<UserRequest> UserRequests { get; set; }
	}
}