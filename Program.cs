using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NutriBest.Server;
using NutriBest.Server.Features.Notifications.Hubs;
using NutriBest.Server.Infrastructure.Extensions;
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

var configuration = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});

configuration.AssertConfigurationIsValid();

var mapper = configuration.CreateMapper();

builder
    .Services
    .AddSingleton(mapper)
    .AddServices()
    .AddHttpContextAccessor()
    .AddMemoryCache()
    .AddResponseCaching()
    .AddDatabase(connectionString)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddIdentity()
    //.AddCors() previous config
    .AddCors(options =>
    {
        options.AddPolicy("MyCorsPolicy", builder =>
        {
            var corsOrigins = applicationSettings.GetSection("AllowedOrigins").Get<string[]>();
            builder.WithOrigins(corsOrigins)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
    })
    .AddJwtAuthentication(secret)
    .Configure<ApplicationSettings>(applicationSettings)
    .AddApiControllers();

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecureSwagger", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting()
    .UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(x => x
//            .AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader()) previous config

app.UseCors("MyCorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/Hubs/Notification");
})
.ApplyMigrations();

//app.UseSwaggerAuthorized(app.Services); gotta uncomment this in the future
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureSwagger v1");
    c.RoutePrefix = string.Empty;
});

app.Run();
