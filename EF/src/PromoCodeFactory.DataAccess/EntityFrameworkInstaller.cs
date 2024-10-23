using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Context;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess;

public static class EntityFrameworkInstaller
{
    public static async Task SeedDatabase(DataContext context)
    {
        var roles = FakeDataFactory.Roles.ToList();
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        var employees = FakeDataFactory.Employees.ToList();
        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();

        var preferences = FakeDataFactory.Preferences.ToList();
        await context.Preferences.AddRangeAsync(preferences);
        await context.SaveChangesAsync();

        var customers = FakeDataFactory.Customers.ToList();
        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        List<CustomerPreference> customerPreferences = new();
        foreach (var customer in customers)
        {
            foreach (var preference in preferences)
            {
                customerPreferences.Add(new CustomerPreference
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    PreferenceId = preference.Id
                });
            }
        }
        await context.CustomerPreferences.AddRangeAsync(customerPreferences);
        await context.SaveChangesAsync();

        var promoCodes = FakeDataFactory.PromoCodes.ToList();
        foreach (var promoCode in promoCodes)
        {
            promoCode.Preference = preferences.First(p => p.Id == promoCode.PreferenceId);
            promoCode.PartnerManager = employees.First(e => e.Id == promoCode.EmployeeId);
            promoCode.Customer = customers.First(c => c.Id == promoCode.CustomerId);
            await context.PromoCodes.AddAsync(promoCode);
        }
        await context.SaveChangesAsync();
    }
}