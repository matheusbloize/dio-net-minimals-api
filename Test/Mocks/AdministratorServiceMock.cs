using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Interfaces;
using dio_net_minimals_api.Domain.DTOs;

namespace Test.Mocks;

public class AdministratorServiceMock : IAdministratorService {
    private static readonly List<Administrator> admins = [
        new() {
            Id = 1,
            Email = "adm@test.com",
            Password = "admin123",
            Profile = "Adm"
        },
        new() {
            Id = 2,
            Email = "editor@test.com",
            Password = "editor123",
            Profile = "Editor"
        }
    ];

    public Administrator? FindById(int id) {
        return admins.Find(a => a.Id == id);
    }

    public Administrator Save(Administrator admin) {
        admin.Id = admins.Count + 1;
        admins.Add(admin);

        return admin;
    }

    public Administrator? Login(LoginDTO loginDTO) {
        return admins.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }

    public List<Administrator> FindAll(int? page) {
        return admins;
    }
}