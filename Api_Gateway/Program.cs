using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(builder.Configuration["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

builder.Configuration.AddJsonFile(Path.Combine("Configuration", "configuration.Development.json"));
builder.Configuration.AddJsonFile(Path.Combine("Registration.Service.Config", "config.Development.json"));
builder.Configuration.AddJsonFile(Path.Combine("Catalog.Service.Config", "config.Development.json"));

builder.Services.AddOcelot();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseAuthentication();

app.UseOcelot();

app.Run();
