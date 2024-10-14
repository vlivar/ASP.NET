using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Context.Configurations;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.FirstName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(e => e.Role)
            .WithMany(r => r.Employees)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
