using System.Reflection;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Services;
using dio_net_minimals_api.Infra.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Test.Domain.Services;

[TestClass]
public class AdministratorServiceTest {
    private static DBContext CreateTestContext() {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DBContext(configuration);
    }

    [TestMethod]
    public void ShouldSaveAdmin() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators");

        var admin = new Administrator {
            Email = "admin@test.com",
            Password = "adminadmin",
            Profile = "Adm"
        };

        var adminService = new AdministratorService(context);

        // Act
        adminService.Save(admin);

        // Assert
        Assert.AreEqual(1, adminService.FindAll(1).Count);
    }

    [TestMethod]
    public void ShouldFindById() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators");

        var admin = new Administrator {
            Email = "admin@test.com",
            Password = "adminadmin",
            Profile = "Adm"
        };

        var adminService = new AdministratorService(context);

        // Act
        adminService.Save(admin);
        var dbAdmin = adminService.FindById(admin.Id);

        // Assert
        Assert.AreEqual(1, dbAdmin!.Id);
    }

    [TestMethod]
    public void ShouldFindAll() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators");

        List<Administrator> admins = [
            new Administrator {
                Email = "admin1@test.com",
                Password = "adminadmin",
                Profile = "Adm"
            },
            new Administrator {
                Email = "admin2@test.com",
                Password = "123",
                Profile = "Adm"
            },
            new Administrator {
                Email = "admin3@test.com",
                Password = "admin1234",
                Profile = "Adm"
            }
        ];

        var adminService = new AdministratorService(context);


        // Act
        foreach (var admin in admins) {
            adminService.Save(admin);
        }
        var dbAdmins = adminService.FindAll(1);

        // Assert
        Assert.AreEqual(3, dbAdmins.Count);
    }
}