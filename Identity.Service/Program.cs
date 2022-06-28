using Identity.Service.Data;
using Identity.Service.JwtService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> Current Environment: Development");
    var connectionString = builder.Configuration.GetConnectionString("DevPsqlConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
}
else if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Current Environment: Production");
    var host = Environment.GetEnvironmentVariable("DB_Host");
    var port = Environment.GetEnvironmentVariable("DB_Port");
    var database = Environment.GetEnvironmentVariable("DB_Name");
    var user = Environment.GetEnvironmentVariable("DB_User");
    var password = Environment.GetEnvironmentVariable("DB_Password");
    var connectionString = $"Host={host};Port={port};Database={database};User Id={user};Password={password}";
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
}

builder.Services.AddAutoMapper(typeof(Program));

var key = builder.Configuration["SecretKey"];
Console.WriteLine($"--> key={key}");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSingleton<IJwtService>(new JwtService(key));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
