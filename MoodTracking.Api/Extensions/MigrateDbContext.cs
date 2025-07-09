using System; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting; 
using Microsoft.Extensions.Logging; 
using Polly; 
/// <summary>
/// Extensões para facilitar a migração automática do banco de dados ao iniciar o host.
/// </summary>
public static class MigrateDbContextExtensionClass 
{ 
    /// <summary>
    /// Executa a migração do banco de dados para o contexto especificado ao iniciar o host.
    /// </summary>
    /// <typeparam name="TContext">Tipo do DbContext a ser migrado.</typeparam>
    /// <param name="host">Instância do host da aplicação.</param>
    /// <returns>O host da aplicação após a migração do banco de dados.</returns>
    public static IHost MigrateDbContext<TContext>(this IHost host) 
        where TContext : DbContext 
    { 
        using (var scope = host.Services.CreateScope()) 
        { 
            var services = scope.ServiceProvider; 
            var logger = services.GetRequiredService<ILogger<TContext>>(); 
            var context = services.GetService<TContext>(); 
            try 
            { 
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}"); 
                var retry = Policy.Handle<Exception>().WaitAndRetry(new[] 
                { 
                    TimeSpan.FromSeconds(5), 
                    TimeSpan.FromSeconds(10), 
                    TimeSpan.FromSeconds(15), 
                }); 
                retry.Execute(() => 
                { 
                    if (context != null && context.Database.CanConnect()) 
                        context.Database.Migrate(); 
                }); 
                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}"); 
            } 
            catch (Exception ex) 
            { 
                logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}"); 
            } 
        } 
        return host; 
    } 
}
