using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Context.Configurations;
internal class CustomerPreferenceConfiguration : IEntityTypeConfiguration<CustomerPreference>
{
    public void Configure(EntityTypeBuilder<CustomerPreference> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.HasOne(cp => cp.Customer)
               .WithMany(c => c.CustomerPreferences)
               .HasForeignKey(cp => cp.CustomerId);

        builder.HasOne(cp => cp.Preference)
               .WithMany(p => p.CustomerPreferences)
               .HasForeignKey(cp => cp.PreferenceId);
    }
}
