using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Interfaces;

namespace Test.Mocks;

public class VehicleServiceMock : IVehicleService {
    private static readonly List<Vehicle> vehicles = [
        new() {
            Id = 1,
            Name = "Opala",
            Make = "Chevrolet",
            Year = 1975
        },
        new() {
            Id = 2,
            Name = "Mustang",
            Make = "Ford",
            Year = 1969
        }
    ];

    public Vehicle? FindById(int id) {
        return vehicles.Find(a => a.Id == id);
    }

    public void Save(Vehicle vehicle) {
        vehicle.Id = vehicles.Count + 1;
        vehicles.Add(vehicle);
    }

    public List<Vehicle> FindAll(int? page = 1, string? name = null, string? make = null) {
        return vehicles;
    }

    public void Update(Vehicle vehicle) {
        string newName = vehicle.Name += " 2.0";
        vehicle.Name = newName;
    }

    public void Delete(Vehicle vehicle) {
        vehicles.Remove(vehicle);
    }
}