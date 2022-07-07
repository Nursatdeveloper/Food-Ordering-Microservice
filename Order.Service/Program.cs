using Microsoft.EntityFrameworkCore;
using Order.Service.AsyncDataServices;
using Order.Service.Data;
using Order.Service.EventProcessing;
using Order.Service.Hubs;
using Order.Service.Models;
using Order.Service.Repository;
using Order.Service.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection to Database
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

builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddScoped<IRepository<FoodOrder>, Repository<FoodOrder>>();
builder.Services.AddScoped<IRepository<OrderStreamingConnection>, Repository<OrderStreamingConnection>>();


builder.Services.AddSignalR();
builder.Services.AddSingleton<IDictionary<string, ConnectionParameters>>(options => new Dictionary<string, ConnectionParameters>());
builder.Services.AddSingleton<IDictionary<string, DeliveryConnection>>(options => new Dictionary<string, DeliveryConnection>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<OrderHub>("/ws/orders");
});

app.Run();
