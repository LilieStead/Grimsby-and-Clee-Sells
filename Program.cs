using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Repositories;
using Grimsby_and_Clee_Sells.UserSession;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<GacsDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var sercetConfig = new SecretKeyGen();

builder.Services.AddSingleton(sercetConfig);

builder.Configuration.Bind("SercetConfig", new SecretKeyGen());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sercetConfig.SecretKey)),
            ValidIssuer = "GrimsbyAndCleeSells",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddScoped<DecodeJWT>();

//link repos to controller
builder.Services.AddScoped<IUserRepository,SQLUserRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();

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

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
