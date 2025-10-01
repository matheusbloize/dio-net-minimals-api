using System.Net;
using System.Text;
using System.Text.Json;
using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Domain.ModelViews;

namespace Test.Requests;

[TestClass]
public class AdministratorRequestTest {
    [ClassInitialize]
    public static void ClassInit(TestContext testContext) {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup() {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task ShouldGetAndSetProperties() {
        // Arrange
        var loginDTO = new LoginDTO {
            Email = "adm@test.com",
            Password = "admin123"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/admin/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdmin = JsonSerializer.Deserialize<LoggedAdmin>(result, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdmin?.Email ?? "");
        Assert.IsNotNull(loggedAdmin?.Profile ?? "");
        Assert.IsNotNull(loggedAdmin?.Token ?? "");
    }
}