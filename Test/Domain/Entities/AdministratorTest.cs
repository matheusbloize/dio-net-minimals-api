using dio_net_minimals_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorTest {
    [TestMethod]
    public void ShouldGetAndSetProperties() {
        // Arrange
        var admin = new Administrator {
            // Act
            Id = 1,
            Email = "admin@test.com",
            Password = "adminadmin",
            Profile = "Adm"
        };

        // Assert
        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("admin@test.com", admin.Email);
        Assert.AreEqual("adminadmin", admin.Password);
        Assert.AreEqual("Adm", admin.Profile);
    }
}