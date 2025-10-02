using System.Net;
using System.Text;
using System.Text.Json;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.ModelViews;

namespace Test.Requests;

[TestClass]
public class VehicleRequestTest {
    [ClassInitialize]
    public static void ClassInit(TestContext testContext) {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup() {
        Setup.ClassCleanup();
    }

    [TestInitialize]
    public async Task TestInit() {
        await AuthenticateAsAdmin();
    }

    private async Task AuthenticateAsAdmin() {
        var loginDTO = new {
            Email = "adm@test.com",
            Password = "admin123"
        };

        var json = JsonSerializer.Serialize(loginDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await Setup.client.PostAsync("/admin/login", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Admin login failed.");

        var result = await response.Content.ReadAsStringAsync();

        var loggedAdmin = JsonSerializer.Deserialize<LoggedAdmin>(result, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdmin?.Token, "Token is null");

        Setup.client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loggedAdmin.Token);
    }

    [TestMethod]
    public async Task ShouldFindAll() {
        // Act
        var response = await Setup.client.GetAsync("/vehicles");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(content, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(vehicles);
        Assert.IsTrue(vehicles.Count >= 0);
    }

    [TestMethod]
    public async Task ShouldSave() {
        // Arrange
        var newVehicle = new Vehicle {
            Name = "Fusca",
            Make = "Volkswagen",
            Year = 1984
        };

        var json = JsonSerializer.Serialize(newVehicle);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PostAsync("/vehicles", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldGetVehicleById() {
        // Act
        var response = await Setup.client.GetAsync("/vehicles/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<Vehicle>(content, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(vehicle);
        Assert.AreEqual(1, vehicle.Id);
    }

    [TestMethod]
    public async Task ShouldUpdate() {
        // Arrange
        var updatedVehicle = new Vehicle {
            Id = 1,
            Name = "Opala SS",
            Make = "Chevrolet",
            Year = 1975
        };

        var json = JsonSerializer.Serialize(updatedVehicle);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PutAsync("/vehicles/1", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldDelete() {
        // Act
        var response = await Setup.client.DeleteAsync("/vehicles/1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}