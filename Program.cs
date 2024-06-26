namespace NutriBest.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startUp = new Startup(builder.Configuration);
            startUp.Configure(builder.Services);

            var app = builder.Build();

            startUp.ConfigureMiddleware(app);
            
            app.Run();
        }
    }
}