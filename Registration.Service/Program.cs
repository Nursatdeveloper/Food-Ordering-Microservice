using Microsoft.EntityFrameworkCore;
using Registration.Service.AsyncDataServices;
using Registration.Service.Data;
using Registration.Service.Models;
using Registration.Service.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection to Database

if(builder.Environment.IsDevelopment())
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

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddScoped<IRepository<Restaurant>, Repository<Restaurant>>();
builder.Services.AddScoped<IRepository<Address>, Repository<Address>>();
builder.Services.AddScoped<IRepository<FoodCategory>, Repository<FoodCategory>>();
builder.Services.AddScoped<IRepository<Food>, Repository<Food>>();
builder.Services.AddScoped<IRepository<OrderStreamingConnection>, Repository<OrderStreamingConnection>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
