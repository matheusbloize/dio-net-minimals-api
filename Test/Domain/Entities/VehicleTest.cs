using dio_net_minimals_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest {
    [TestMethod]
    public void ShouldGetAndSetProperties() {
        // Arrange
        var vehicle = new Vehicle {
            // Act
            Id = 1,
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        };

        // Assert
        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Opala", vehicle.Name);
        Assert.AreEqual("Chevrolet", vehicle.Make);
        Assert.AreEqual(1975, vehicle.Year);
    }
}