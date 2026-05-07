using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShopEgypt.Data.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var webProjectPath = Path.GetFullPath(Path.Combine(basePath, "..", "ShopEgypt"));
            var appSettingsPath = Path.Combine(webProjectPath, "appsettings.json");
            var appSettingsDevelopmentPath = Path.Combine(
                webProjectPath,
                "appsettings.Development.json"
            );

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: false)
                .AddJsonFile(appSettingsDevelopmentPath, optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found in ShopEgypt appsettings."
                );
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
