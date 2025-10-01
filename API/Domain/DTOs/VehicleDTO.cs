namespace dio_net_minimals_api.Domain.DTOs;

public record VehicleDTO {
    public string Name { get; set; } = default!;
    public string Make { get; set; } = default!;
    public int Year { get; set; } = default!;
}