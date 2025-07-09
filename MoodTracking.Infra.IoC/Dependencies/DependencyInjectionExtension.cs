using System; 
using System.Reflection; 
using Infra.Data.Contexts; 
using MediatR; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 
using Serilog; 
using MoodTracking.Infra.Data.Repositories;
using Domain.Interfaces.Repositories;
namespace Infra.IoC 
{ 
    public static class DependencyInjectionExtension 
    { 
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        { 
            services.AddDbContext<ApplicationDbContext>(); 
            services.AddScoped<IGenericRepository<MoodTracking.Domain.Entities.MoodEntry>, MoodEntryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddMediatR(AppDomain.CurrentDomain.Load("MoodTracking.Application")); 
            services.AddAutoMapper(AppDomain.CurrentDomain.Load("MoodTracking.Application")); 
            Log.Logger = new LoggerConfiguration() 
               .WriteTo.Console() 
               .WriteTo.File("./logs/servicelog-.log", rollingInterval: RollingInterval.Day) 
               .CreateLogger(); 
            return services; 
        } 
    } 
}
