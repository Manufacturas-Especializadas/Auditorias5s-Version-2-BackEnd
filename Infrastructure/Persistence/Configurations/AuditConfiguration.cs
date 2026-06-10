using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.ToTable("Audits");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FinalScore)
                .HasPrecision(5, 2);

            builder.Property(a => a.Verdict)
                .HasMaxLength(100);

            builder.HasOne(a => a.Area)
                .WithMany(ar => ar.Audits)
                .HasForeignKey(a => a.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Auditor)
                .WithMany(au => au.Audits)
                .HasForeignKey(a => a.AuditorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}