using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TCS_Cliente.Data;
using TCS_Cliente.Services;
using Microsoft.AspNetCore.Mvc;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                // Aqui configuramos as URLs diretamente no WebHostBuilder
                webBuilder.UseUrls("http://localhost:21232", "https://localhost:21233");

                webBuilder.ConfigureServices((context, services) =>
                {
                    // Registrando o servi�o UserService
                    services.AddScoped<IUserService, UserService>();

                    // Adicionando o contexto do banco de dados
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=app.db"));

                    // Configura��o de autentica��o JWT
                    var jwtKey = context.Configuration["Jwt:Key"] ?? "default_secret_key"; // Lendo chave de configura��es ou vari�vel de ambiente

                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                                ClockSkew = TimeSpan.Zero
                            };
                        });

                    // Configura��o do Swagger
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCS Cliente API", Version = "v1" });
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
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
                                Array.Empty<string>()
                            }
                        });
                    });

                    services.AddControllers()
                        .ConfigureApiBehaviorOptions(options =>
                        {
                            options.InvalidModelStateResponseFactory = context =>
                                new BadRequestObjectResult(new { mensagem = "Dados inv�lidos" });
                        });
                });

                webBuilder.Configure((context, app) =>
                {
                    var env = context.HostingEnvironment;

                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/error");
                    }

                    app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                    // Caso queira for�ar HTTPS em produ��o, descomente a linha abaixo
                    // app.UseHttpsRedirection();

                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();

                    // Configura��o do Swagger
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TCS Cliente API v1");
                        c.RoutePrefix = "swagger";
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            });
}
