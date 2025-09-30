namespace dio_net_minimals_api.Domain.ModelViews;

public record AdminView {
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
}