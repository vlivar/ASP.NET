using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Context.Configurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.FirstName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(50)
            .IsRequired();
    }
}
