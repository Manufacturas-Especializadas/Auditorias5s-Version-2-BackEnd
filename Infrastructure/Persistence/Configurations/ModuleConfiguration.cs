using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Domain.Entities.Module>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Module> builder)
        {
            builder.ToTable("Modules");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}