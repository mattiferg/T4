namespace T4;

public static class Program
{
    public static void Main(string[] args)
    {
        RunWebApplication(args);
    }

    private static void RunWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;
        var configuration = builder.Configuration;

        AuthPrep.ConfigureAuthentication(services, configuration);
        AxClient.AxClientRegistrar.RegisterServices(services, configuration);

        services.AddAuthorization();
        services.AddOpenApi();
        services.AddControllers().AddNewtonsoftJson();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7235")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "AxpPortal API");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}