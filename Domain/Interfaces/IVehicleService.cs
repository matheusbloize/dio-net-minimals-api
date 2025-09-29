using dio_net_minimals_api.Domain.Entities;

namespace dio_net_minimals_api.Domain.Interfaces;

public interface IVehicleService {
    List<Vehicle> FindAll(int? page = 1, string? name = null, string? make = null);
    Vehicle? FindById(int id);
    void Save(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vehicle);
}