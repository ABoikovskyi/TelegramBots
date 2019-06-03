using DataLayer.Models.Festival;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class FestivalDbContext : DbContext
    {
        public FestivalDbContext(DbContextOptions<FestivalDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Festival> Festivals { get; set; }
        public virtual DbSet<Stage> Stages { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
    }
}