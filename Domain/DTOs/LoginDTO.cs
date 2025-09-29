namespace dio_net_minimals_api.Domain.DTOs;

public class LoginDTO {
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}