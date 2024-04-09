using Microsoft.EntityFrameworkCore;
using NutriBest.Server;
using NutriBest.Server.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder
    .Configuration
    .GetConnectionString("DefaultConnection");

var applicationSettings = builder
    .Configuration
    .GetSection("ApplicationSettings");

var appSettings = applicationSettings
    .Get<ApplicationSettings>();

var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

builder
    .Services
    .AddDatabase(connectionString)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddIdentity()
    .AddCors()
    .AddJwtAuthentication(secret)
    .Configure<ApplicationSettings>(applicationSettings)
    .AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication();

app.UseAuthorization();

app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader())
    .UseEndpoints(endpoints => endpoints.MapControllers())
    .ApplyMigrations();

app.Run();
