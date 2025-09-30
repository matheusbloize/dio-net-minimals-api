using System.Data.Common;
using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Interfaces;
using dio_net_minimals_api.Infra.DB;

namespace dio_net_minimals_api.Domain.Services;

public class AdministratorService : IAdministratorService {
    private readonly DBContext _context;

    public AdministratorService(DBContext context) {
        _context = context;
    }

    public List<Administrator> FindAll(int? page) {
        var query = _context.Administrators.AsQueryable();

        int itemsPerPage = 10;

        if (page != null) {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return [.. query];
    }

    public Administrator? Login(LoginDTO loginDTO) {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }

    public Administrator Save(Administrator admin) {
        _context.Administrators.Add(admin);
        _context.SaveChanges();

        return admin;
    }

    public Administrator? FindById(int id) {
        return _context.Administrators.Where(v => v.Id == id).FirstOrDefault();
    }
}