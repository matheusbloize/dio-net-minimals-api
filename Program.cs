using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Enums;
using dio_net_minimals_api.Domain.Interfaces;
using dio_net_minimals_api.Domain.ModelViews;
using dio_net_minimals_api.Domain.Services;
using dio_net_minimals_api.Infra.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Admin
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null) {
        return Results.Ok("Login com sucesso");
    } else {
        return Results.Unauthorized();
    }
}).WithTags("Admin");

app.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService) => {
    var admins = administratorService.FindAll(page);
    var adminsView = new List<AdminView>();

    foreach (var adm in admins) {
        adminsView.Add(new AdminView {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }

    return Results.Ok(adminsView);
}).WithTags("Admin");

app.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) => {
    var admin = administratorService.FindById(id);

    if (admin == null) return Results.NotFound("Admin not found!");

    return Results.Ok(new AdminView {
        Id = admin.Id,
        Email = admin.Email,
        Profile = admin.Profile
    });
}).WithTags("Admin");

app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdministratorService administratorService) => {
    var validation = new ValidationErrors {
        Messages = []
    };

    if (string.IsNullOrEmpty(adminDTO.Email)) {
        validation.Messages.Add("Email cannot be empty!");
    }
    if (string.IsNullOrEmpty(adminDTO.Password)) {
        validation.Messages.Add("Password cannot be empty!");
    }
    if (adminDTO.Profile == null) {
        validation.Messages.Add("Profile cannot be empty!");
    }

    if (validation.Messages.Count > 0) {
        return Results.BadRequest(validation);
    }

    var admin = new Administrator {
        Email = adminDTO.Email,
        Password = adminDTO.Password,
        Profile = adminDTO.Profile.ToString() ?? Profile.Editor.ToString(),
    };
    administratorService.Save(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminView {
        Id = admin.Id,
        Email = admin.Email,
        Profile = admin.Profile
    });
}).WithTags("Admin");
#endregion

#region Vehicles
ValidationErrors validDTO(VehicleDTO vehicleDTO) {
    var validation = new ValidationErrors {
        Messages = []
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name)) {
        validation.Messages.Add("Name cannot be empty!");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Make)) {
        validation.Messages.Add("Make cannot be empty!");
    }

    if (vehicleDTO.Year < 1950) {
        validation.Messages.Add("Vehicle too old!");
    }

    return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    var validation = validDTO(vehicleDTO);

    if (validation.Messages.Count > 0) {
        return Results.BadRequest(validation);
    }

    var vehicle = new Vehicle {
        Name = vehicleDTO.Name,
        Make = vehicleDTO.Make,
        Year = vehicleDTO.Year,
    };
    vehicleService.Save(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
    var vehicles = vehicleService.FindAll(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound("Vehicle not found!");

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound("Vehicle not found!");

    var validation = validDTO(vehicleDTO);

    if (validation.Messages.Count > 0) {
        return Results.BadRequest(validation);
    }

    vehicle.Name = vehicleDTO.Name;
    vehicle.Make = vehicleDTO.Make;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.FindById(id);

    if (vehicle == null) return Results.NotFound("Vehicle not found!");

    vehicleService.Delete(vehicle);

    return Results.Ok("Vehicle deleted!");
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion