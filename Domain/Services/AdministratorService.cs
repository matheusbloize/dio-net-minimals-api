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

    public Administrator? Login(LoginDTO loginDTO) {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
}