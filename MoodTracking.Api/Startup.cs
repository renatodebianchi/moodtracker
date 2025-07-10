using Api.Hubs; 
using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting; 
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using System.Text;
using Infra.IoC; 
using System; 
using System.IO; 
using System.Reflection; 
namespace MoodTracking.Api 
{ 
    /// <summary>
    /// Classe de inicialização da aplicação ASP.NET Core.
    /// </summary>
    public class Startup 
    { 
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">Configuração da aplicação.</param>
        public Startup(IConfiguration configuration) 
        { 
            Configuration = configuration; 
        } 
        /// <summary>
        /// Obtém a configuração da aplicação.
        /// </summary>
        public IConfiguration Configuration { get; } 
        /// <summary>
        /// Método chamado pela runtime para adicionar serviços ao contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        public void ConfigureServices(IServiceCollection services) 
        { 
            services.AddInfrastructure(Configuration); 
            services.AddControllers(); 
            services.AddSignalR() 
                    .AddJsonProtocol(options => 
                    { 
                       options.PayloadSerializerOptions.PropertyNamingPolicy = null; 
                    }); 
			 services.AddCors(options => options.AddPolicy("CorsPolicy", 
            builder => 
            { 
                builder.AllowAnyHeader() 
                        .AllowAnyMethod() 
                        .SetIsOriginAllowed((host) => true) 
                        .AllowCredentials(); 
            })); 
            services.AddSwaggerGen(c => 
            { 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoodTracking.Api", Version = "v1" }); 
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

            }); 
            var jwtKey = Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key não configurada. Defina Jwt:Key no appsettings.json.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        } 
        /// <summary>
        /// Configura o pipeline de requisições HTTP da aplicação.
        /// </summary>
        /// <param name="app">Construtor de aplicações.</param>
        /// <param name="env">Informações sobre o ambiente de hospedagem.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        { 
            //if (env.IsDevelopment()) 
            //{ 
                app.UseDeveloperExceptionPage(); 
                app.UseSwagger(); 
                app.UseSwaggerUI(c => { 
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoodTracking.Api v1"); 
                }); 
            //} 
            app.UseHttpsRedirection(); 
            app.UseCors("CorsPolicy"); 
            app.UseStaticFiles(); 
            app.UseRouting(); 
            app.UseAuthentication();
            app.UseAuthorization(); 
            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllers(); 
                endpoints.MapHub<WebSocketHub>("/Hub"); 
            }); 
        } 
    } 
}
