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

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    if(hostingContext.HostingEnvironment.IsDevelopment())
    {
        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddOcelot($"{hostingContext.HostingEnvironment.ContentRootPath}/DevelopmentConfigurations", hostingContext.HostingEnvironment)
            .AddEnvironmentVariables();
    }
    else
    {
        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddOcelot($"{hostingContext.HostingEnvironment.ContentRootPath}/ProductionConfigurations", hostingContext.HostingEnvironment)
            .AddEnvironmentVariables();
    }

});

builder.Services.AddOcelot();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseAuthentication();

app.UseOcelot();

app.Run();
