using BusTracker.Data;
using BusTracker.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.WebHost.UseUrls("http://0.0.0.0:5262");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- Middleware ---
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

// --- Endpoints ---
app.MapControllers();
app.MapHub<PositionsHub>("/positionshub"); 

app.Run();
