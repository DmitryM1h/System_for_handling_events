using Microsoft.EntityFrameworkCore;
using EventProcessor.Models;

namespace EventProcessor.db_context
{
    public class IncidentContext: DbContext
    {
        public IncidentContext(DbContextOptions<IncidentContext> options):base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Incident> Incidents { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;



    }
}
