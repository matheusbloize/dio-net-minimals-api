using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Domain.Entities;

namespace dio_net_minimals_api.Domain.Interfaces;

public interface IAdministratorService {
    Administrator? Login(LoginDTO loginDTO);
}