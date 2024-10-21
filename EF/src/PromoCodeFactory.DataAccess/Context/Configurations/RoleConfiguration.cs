using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Context.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(100);
    }
}
