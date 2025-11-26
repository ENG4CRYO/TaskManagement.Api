using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
         
            var connectionString = configuration.GetConnectionString("DefaultConnection");

           
            services.AddDbContext<TaskDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(ServiceCollectionExtensions).Assembly.FullName) 
                ));
            services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
        }
    }

