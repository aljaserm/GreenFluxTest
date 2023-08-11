using Microsoft.EntityFrameworkCore;

namespace GreenFlux_V15.Data
{
    public class InMemoryChargingDbContext : ChargingDbContext
    {
        public InMemoryChargingDbContext(DbContextOptions<ChargingDbContext> options) : base(options)
        {
        }
    }
}
