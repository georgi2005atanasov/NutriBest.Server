namespace NutriBest.Server
{
    using AutoMapper;
    using Microsoft.OpenApi.Models;
    using NutriBest.Server.Features.Notifications.Hubs;
    using NutriBest.Server.Infrastructure.Extensions;
    using System.Text;

    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
            => this.Configuration = configuration;

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
                .AddSingleton(mapper)
                .AddServices()
                .AddHttpContextAccessor()
                .AddMemoryCache()
                .AddResponseCaching()
                .AddDatabase(connectionString)
                .AddDatabaseDeveloperPageExceptionFilter()
                .AddIdentity()
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

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("MyCorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/Hubs/Notification");
            });

            app.ApplyMigrations();

            // app.UseSwaggerAuthorized(app.Services); // Uncomment this in the future
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureSwagger v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
