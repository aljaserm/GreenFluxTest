using GreenFlux_V15.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux_V15.Data
{
    public class ChargingDbContext : DbContext
    {
        public ChargingDbContext(DbContextOptions<ChargingDbContext> options) : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<ChargeStation> ChargeStations { get; set; }
        public DbSet<Connector> Connectors { get; set; }
    }
}
