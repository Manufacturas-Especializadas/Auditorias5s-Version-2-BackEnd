using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditAnswerConfiguration : IEntityTypeConfiguration<AuditAnswer>
    {
        public void Configure(EntityTypeBuilder<AuditAnswer> builder)
        {
            builder.ToTable("AuditAnswers");
            builder.HasKey(aa => aa.Id);

            builder.ToTable(t => t.HasCheckConstraint("CK_AuditAnswers_Score", "[Score] BETWEEN 1 AND 5"));

            builder.HasOne(aa => aa.Audit)
                .WithMany(a => a.Answers)
                .HasForeignKey(aa => aa.AuditId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(aa => aa.Question)
                .WithMany(q => q.AuditAnswers)
                .HasForeignKey(aa => aa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
