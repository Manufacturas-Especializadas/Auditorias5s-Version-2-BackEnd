using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Module> Modules => Set<Domain.Entities.Module>();

        public DbSet<Area> Areas => Set<Area>();

        public DbSet<Category5s> Categories5S => Set<Category5s>();

        public DbSet<Auditor> Auditors => Set<Auditor>();

        public DbSet<Question> Questions => Set<Question>();

        public DbSet<Audit> Audits => Set<Audit>();

        public DbSet<AuditAnswer> AuditAnswers => Set<AuditAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}