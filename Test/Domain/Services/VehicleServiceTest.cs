using System.Reflection;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Services;
using dio_net_minimals_api.Infra.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Test.Domain.Services;

[TestClass]
public class VehicleServiceTest {
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
    public void ShouldSaveVehicle() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        var vehicle = new Vehicle {
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Save(vehicle);

        // Assert
        Assert.AreEqual(1, vehicleService.FindAll(1).Count);
    }

    [TestMethod]
    public void ShouldFindById() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        var vehicle = new Vehicle {
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Save(vehicle);
        var dbVehicle = vehicleService.FindById(vehicle.Id);

        // Assert
        Assert.AreEqual(1, dbVehicle!.Id);
    }

    [TestMethod]
    public void ShouldFindAll() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        List<Vehicle> vehicles = [
            new Vehicle {
                Name = "Opala",
                Make = "Chevrolet",
                Year = 1975
            },
            new Vehicle {
                 Name = "Fusca",
                Make = "Volkswagen",
                Year = 1995
            },
            new Vehicle {
                 Name = "Chevette ",
                Make = "Chevrolet",
                Year = 1986
            }
        ];

        var vehicleService = new VehicleService(context);


        // Act
        foreach (var vehicle in vehicles) {
            vehicleService.Save(vehicle);
        }
        var dbVehicles = vehicleService.FindAll(1);

        // Assert
        Assert.AreEqual(3, dbVehicles.Count);
    }

    [TestMethod]
    public void ShouldDelete() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        var vehicle = new Vehicle {
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Save(vehicle);
        vehicleService.Delete(vehicle);
        var dbVehicle = vehicleService.FindById(vehicle.Id);

        // Assert
        Assert.AreEqual(null, dbVehicle);
    }

    [TestMethod]
    public void ShouldUpdate() {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        var vehicle = new Vehicle {
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        };

        var newVehicle = new Vehicle {
            Name = "Opala 2.0",
            Make = "Chevrolet",
            Year = 1978
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Save(vehicle);
        var dbVehicle = vehicleService.FindById(vehicle.Id);

        vehicle.Name = newVehicle.Name;
        vehicle.Make = newVehicle.Make;
        vehicle.Year = newVehicle.Year;

        vehicleService.Update(vehicle);

        // Assert
        Assert.AreEqual(vehicle.Id, dbVehicle!.Id);
        Assert.AreEqual("Opala 2.0", dbVehicle!.Name);
        Assert.AreEqual("Chevrolet", dbVehicle!.Make);
        Assert.AreEqual(1978, dbVehicle!.Year);
    }
}