using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Interfaces;
using dio_net_minimals_api.Infra.DB;
using Microsoft.EntityFrameworkCore;

namespace dio_net_minimals_api.Domain.Services;

public class VehicleService : IVehicleService {
    private readonly DBContext _context;

    public VehicleService(DBContext context) {
        _context = context;
    }

    public List<Vehicle> FindAll(int? page = 1, string? name = null, string? make = null) {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(name)) {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
        }

        int itemsPerPage = 10;

        if (page != null) {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return [.. query];
    }

    public void Delete(Vehicle vehicle) {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? FindById(int id) {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Save(Vehicle vehicle) {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void Update(Vehicle vehicle) {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}