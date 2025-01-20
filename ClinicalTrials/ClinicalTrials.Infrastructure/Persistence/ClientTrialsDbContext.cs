using ClinicalTrials.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrials.Infrastructure.Persistence
{
    public class ClientTrialsDbContext : DbContext
    {
        public ClientTrialsDbContext() { }
        public ClientTrialsDbContext(DbContextOptions<ClientTrialsDbContext> options)
        : base(options) { }

        public virtual DbSet<Trial> ClinicalTrials { get; set; }
    }
}
