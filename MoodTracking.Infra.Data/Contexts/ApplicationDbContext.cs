using Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using MoodTracking.Domain.Entities;
using MoodTracking.Domain.ValueObjects;

namespace Infra.Data.Contexts 
{ 
    public class ApplicationDbContext : DbContext 
    { 
        private readonly IConfiguration _configuration; 
        public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
            _configuration = configuration; 
        } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        { 
            optionsBuilder.UseSqlite( 
                _configuration.GetConnectionString("DefaultConnectionSqlite"), 
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName) 
            ); 
        } 
        protected override void OnModelCreating(ModelBuilder builder) 
        { 
            base.OnModelCreating(builder); 
            // Remover chamada se não houver classes de configuração customizadas
            // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            var adminId = new Guid("11111111-1111-1111-1111-111111111111");
            builder.Entity<User>().HasData(new User {
                Id = adminId,
                Nome = "admin",
                Email = "admin@mood.com"
            });
        } 
        public DbSet<MoodEntry> MoodEntries { get; set; }
        public DbSet<User> Users { get; set; }
    } 
}
