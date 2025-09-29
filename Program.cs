using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Infra.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DBContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection")));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@test.com" && loginDTO.Password == "admin123") {
        return Results.Ok("Login com sucesso");
    } else {
        return Results.Unauthorized();
    }
});

app.Run();