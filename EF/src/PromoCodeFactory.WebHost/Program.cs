using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Context;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.MigrateDatabaseAsync();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}

public static class HostExtensions
{
    public static async Task MigrateDatabaseAsync(this IHost webHost)
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            using (var context = services.GetRequiredService<DataContext>())
            {
                try
                {
                    context.Database.EnsureDeleted();

                    await context.Database.MigrateAsync();
                    await EntityFrameworkInstaller.SeedDatabase(context);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}