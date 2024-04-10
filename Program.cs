using Microsoft.EntityFrameworkCore;
using NutriBest.Server;
using NutriBest.Server.Features.Identity;
using NutriBest.Server.Features.Products;
using NutriBest.Server.Infrastructure.Extensions;
using NutriBest.Server.Infrastructure.Filters;
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

//make extension method for adding services
builder.Services
    .AddTransient<IIdentityService, IdentityService>()
    .AddTransient<IProductService, ProductService>();

builder
    .Services
    .AddDatabase(connectionString)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddIdentity()
    .AddCors()
    .AddJwtAuthentication(secret)
    .Configure<ApplicationSettings>(applicationSettings)
    .AddApiControllers();

builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
    c.RoutePrefix = string.Empty;
});

app.Run();
