using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var authority = builder.Configuration["Authentication:Authority"];
var audience = builder.Configuration["Authentication:Audience"];

if (string.IsNullOrWhiteSpace(authority))
{
    throw new InvalidOperationException("Authentication:Authority must be configured.");
}

if (!Uri.TryCreate(authority, UriKind.Absolute, out _))
{
    throw new InvalidOperationException("Authentication:Authority must be configured as an absolute URI.");
}

if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("Authentication:Audience must be configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
