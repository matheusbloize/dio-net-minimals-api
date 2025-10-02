using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dio_net_minimals_api.Domain.DTOs;
using dio_net_minimals_api.Domain.Entities;
using dio_net_minimals_api.Domain.Enums;
using dio_net_minimals_api.Domain.Interfaces;
using dio_net_minimals_api.Domain.ModelViews;
using dio_net_minimals_api.Domain.Services;
using dio_net_minimals_api.Infra.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Startup {
    private string Key { get; set; }
    public IConfiguration Configuration { get; set; } = default!;

    public Startup(IConfiguration configuration) {
        Configuration = configuration;
        Key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    public void ConfigureServices(IServiceCollection services) {
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key ?? "")),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert the JWT token here"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddDbContext<DBContext>(options => {
            options.UseMySql(Configuration.GetConnectionString("MySqlConnection"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("MySqlConnection")));
        });

        services.AddCors(options => {
            options.AddDefaultPolicy(
                builder => {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints => {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admin
            string GenerateJwtToken(Administrator admin) {
                if (string.IsNullOrEmpty(Key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>() {
                    new("Email", admin.Email),
                    new("Profile", admin.Profile),
                    new(ClaimTypes.Role, admin.Profile),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
                var admin = administratorService.Login(loginDTO);
                if (admin != null) {
                    string token = GenerateJwtToken(admin);
                    return Results.Ok(new LoggedAdmin {
                        Email = admin.Email,
                        Profile = admin.Profile,
                        Token = token,
                    });
                } else {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Admin");

            endpoints.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService) => {
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");

            endpoints.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) => {
                var admin = administratorService.FindById(id);

                if (admin == null) return Results.NotFound("Admin not found!");

                return Results.Ok(new AdminView {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");

            endpoints.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdministratorService administratorService) => {
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");
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

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
                var vehicles = vehicleService.FindAll(page);

                return Results.Ok(vehicles);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.FindById(id);

                if (vehicle == null) return Results.NotFound("Vehicle not found!");

                return Results.Ok(vehicle);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.FindById(id);

                if (vehicle == null) return Results.NotFound("Vehicle not found!");

                vehicleService.Delete(vehicle);

                return Results.Ok("Vehicle deleted!");
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");
            #endregion
        });
    }
}