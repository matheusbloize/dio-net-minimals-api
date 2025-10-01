using dio_net_minimals_api.Domain.Enums;

namespace dio_net_minimals_api.Domain.DTOs;

public record AdminDTO {
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Profile? Profile { get; set; } = default!;
}