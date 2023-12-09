using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<GacsDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//link repos to controller
builder.Services.AddScoped<IUserRepository,SQLUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grimsby and Clee Sells");
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "UI")),
        RequestPath = "/UI"

    });
    var url = "https://192.168.0.135:44394/UI/index.html";
    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
