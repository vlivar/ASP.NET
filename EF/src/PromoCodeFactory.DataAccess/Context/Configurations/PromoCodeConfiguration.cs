using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Context.Configurations;

internal class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.Property(pc => pc.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pc => pc.ServiceInfo)
            .HasMaxLength(100);

        builder.Property(pc => pc.PartnerName)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(pc => pc.PartnerManager)
            .WithMany()
            .HasForeignKey(pc => pc.EmployeeId);

        builder.HasOne(pc => pc.Preference)
            .WithMany()
            .HasForeignKey(pc => pc.PreferenceId);

        builder.HasOne(pc => pc.Customer)
            .WithMany(c => c.PromoCodes)
            .HasForeignKey(pc => pc.CustomerId);
    }
}
