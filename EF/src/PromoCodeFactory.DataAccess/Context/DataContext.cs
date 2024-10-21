using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Reflection;

namespace PromoCodeFactory.DataAccess.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
