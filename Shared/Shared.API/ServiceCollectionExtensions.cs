public static class ServiceCollectionExtensions
{
    public static void AddJaegerTracing(this IServiceCollection serviceCollection, string service) =>
        serviceCollection.AddOpenTelemetry()
            .WithTracing(traceProviderBuilder => traceProviderBuilder
                .AddSource(service)
                .ConfigureResource(resourceBuilder => resourceBuilder.AddService(service))
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddJaegerExporter()
            );
}
