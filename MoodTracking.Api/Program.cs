using Infra.Data.Contexts; 
using MoodTracking.Api; 
using Microsoft.Extensions.DependencyInjection;
using MoodTracking.Domain.Entities;
using Domain.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args); 
var startup = new Startup(builder.Configuration); 
startup.ConfigureServices(builder.Services); 
var app = builder.Build(); 
try 
{  
    app.MigrateDbContext<ApplicationDbContext>();  
    // Seed programático para senha do admin
    using (var scope = app.Services.CreateScope())
    {
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var admin = (await userRepo.GetAllAsync()).FirstOrDefault(u => u.Email == "admin@mood.com");
        if (admin != null)
        {
            // Só define a senha se SenhaProtegida estiver vazia ou nula
            if (string.IsNullOrEmpty(admin.SenhaProtegida))
            {
                Console.WriteLine("Setting password for admin user.");
                admin.DefinirSenha("Admin@135");
                await userRepo.UpdateAsync(admin);
            }
        }
    }
} catch (Exception ex)  
{  
    Console.WriteLine(ex.Message);  
} 
startup.Configure(app, app.Environment); 
app.Run();
