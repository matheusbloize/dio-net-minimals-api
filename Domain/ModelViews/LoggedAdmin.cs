namespace dio_net_minimals_api.Domain.ModelViews;

public record LoggedAdmin {
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
    public string Token { get; set; } = default!;
}