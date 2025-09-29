namespace dio_net_minimals_api.Domain.ModelViews;

public struct Home {
    public string Message { get => "Welcome to the vehicles API!"; }

    public string Doc {
        get => "/swagger";
    }
}