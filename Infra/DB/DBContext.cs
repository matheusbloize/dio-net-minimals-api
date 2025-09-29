using dio_net_minimals_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace dio_net_minimals_api.Infra.DB;

public class DBContext : DbContext {
    private readonly IConfiguration _appSettingsConfig;

    public DBContext(IConfiguration appSettingsConfig) {
        _appSettingsConfig = appSettingsConfig;
    }

    public DbSet<Administrator> Administrators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            var connectionString = _appSettingsConfig.GetConnectionString("MySqlConnection")?.ToString();
            if (!string.IsNullOrEmpty(connectionString)) {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}