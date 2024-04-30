using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NutriBest.Server;
using NutriBest.Server.Features.Admin;
using NutriBest.Server.Features.Carts;
using NutriBest.Server.Features.Categories;
using NutriBest.Server.Features.Identity;
using NutriBest.Server.Features.Images;
using NutriBest.Server.Features.NutritionsFacts;
using NutriBest.Server.Features.Products;
using NutriBest.Server.Features.ProductsDetails;
using NutriBest.Server.Features.Promotions;
using NutriBest.Server.Infrastructure.Extensions;
using NutriBest.Server.Infrastructure.Services;
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
    .AddScoped<ICurrentUserService, CurrentUserService>()
    .AddTransient<IAdminService, AdminService>()
    .AddTransient<IIdentityService, IdentityService>()
    .AddTransient<IProfileService, ProfileService>()
    .AddTransient<IProductService, ProductService>()
    .AddTransient<IProductDetailsService, ProductDetailsService>()
    .AddTransient<INutritionFactsService, NutritionFactsService>()
    .AddTransient<IPromotionService, PromotionService>()
    .AddTransient<ICartService,CartService>()
    .AddTransient<IImageService, ImageService>()
    .AddTransient<ICategoryService, CategoryService>();

builder
    .Services
    .AddHttpContextAccessor()
    .AddMemoryCache()
    .AddDatabase(connectionString)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddIdentity()
    .AddCors()
    .AddJwtAuthentication(secret)
    .Configure<ApplicationSettings>(applicationSettings)
    .AddApiControllers();

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
    .UseAuthentication();

app.UseAuthorization();

app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader())
    .UseEndpoints(endpoints => endpoints.MapControllers())
    .ApplyMigrations();

//app.UseSwaggerAuthorized(app.Services); gotta uncomment this in the future
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureSwagger v1");
    c.RoutePrefix = string.Empty;
});

app.Run();
