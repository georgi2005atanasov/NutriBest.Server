namespace NutriBest.Server
{
    using System.Text;
    using AutoMapper;
    using Microsoft.OpenApi.Models;
    using NutriBest.Server.Features.Notifications.Hubs;
    using NutriBest.Server.Infrastructure.Extensions;

    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var jwtKey = Configuration["ApplicationSettings:Secret"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = KeyGenerator.GenerateRandomKey(32); // 32 bytes for a 256-bit key
                                                             // Store this key in the configuration (example shows in-memory update)
                Configuration["ApplicationSettings:Secret"] = jwtKey;
            }
        }

        public void Configure(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var applicationSettings = Configuration.GetSection("ApplicationSettings");
            var appSettings = applicationSettings.Get<ApplicationSettings>();
            var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            mapperConfiguration.AssertConfigurationIsValid();
            var mapper = mapperConfiguration.CreateMapper();

            services
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
                .AddSingleton(mapper)
                .AddServices()
                .AddHttpContextAccessor()
                .AddMemoryCache()
                .AddResponseCaching()
                .AddDatabase(connectionString)
                .AddDatabaseDeveloperPageExceptionFilter()
                .AddIdentity()
                .AddJwtAuthentication(secret)
                .Configure<ApplicationSettings>(applicationSettings)
                .AddApiControllers();

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecureSwagger", Version = "v1" });
            });
        }

        public void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("MyCorsPolicy");

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseSwaggerAuthorized(app.Services); // Uncomment this in the future
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureSwagger v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/Hubs/Notification");
            });

            app.ApplyMigrations();
        }
    }
}
