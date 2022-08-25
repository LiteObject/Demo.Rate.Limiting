using Demo.Api.Middlewares;
using Demo.Api.Rules;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddPooledDbContextFactory<RequestDbContext>(options =>
            {
                options.UseInMemoryDatabase("HttpRequestDatabase");
                options.LogTo(Console.WriteLine, LogLevel.Information);
            });

            builder.Services.AddSingleton<IRulesManager, RulesManager>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseMiddleware<RateLimitingMiddleware>();
            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// Used to troubleshoot DI lifespan issues :(
        /// </summary>
        /// <param name="services"></param>
        private static void PrintLifetimeOfServices(IServiceCollection services)
        {
            Console.WriteLine("\n>>> SERVICE COLLECTION:");

            var myServices = services.Where(s => s.ServiceType.FullName != null && s.ServiceType.FullName.StartsWith("Demo")).ToList();

            foreach (var s in myServices)
            {
                Console.WriteLine($" >> Service Type: {s.ServiceType.Name} -> Instance Type: {s.ImplementationType?.Name}, Lifetime: {s.Lifetime}");
            }

            Console.WriteLine();
        }
    }
}