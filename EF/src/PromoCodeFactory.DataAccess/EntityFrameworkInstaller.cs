using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Context;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess;

public static class EntityFrameworkInstaller
{
    public static IServiceCollection ConfigureContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DataContext>(optionsBuilder
            => optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlite(connectionString));

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            SeedDatabase(dbContext);
        }

        return services;
    }

    private static void SeedDatabase(DataContext context)
    {
        List<Role> roles = FakeDataFactory.Roles.ToList();
        context.Roles.AddRange(roles);
        context.SaveChanges();

        List<Employee> employees = FakeDataFactory.Employees.ToList();
        context.Employees.AddRange(employees);
        context.SaveChanges();

        List<Preference> preferences = FakeDataFactory.Preferences.ToList();
        context.Preferences.AddRange(preferences);
        context.SaveChanges();

        List<Customer> customers = FakeDataFactory.Customers.ToList();
        context.Customers.AddRange(customers);
        context.SaveChanges();

        List<CustomerPreference> customerPreferences = new();
        foreach (Customer customer in customers)
        {
            foreach (Preference preference in preferences)
            {
                customerPreferences.Add(new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    PreferenceId = preference.Id
                });
            }
        }
        context.CustomerPreferences.AddRange(customerPreferences);
        context.SaveChanges();

        List<PromoCode> promoCodes = FakeDataFactory.PromoCodes.ToList();
        foreach (PromoCode promoCode in promoCodes)
        {
            promoCode.Preference = preferences.First(p => p.Id == promoCode.PreferenceId);
            promoCode.PartnerManager = employees.First(e => e.Id == promoCode.EmployeeId);
            promoCode.Customer = customers.First(c => c.Id == promoCode.CustomerId);
            context.PromoCodes.Add(promoCode);
        }
        context.SaveChanges();
    }
}
