using TCS_Cliente.Services;
using Microsoft.EntityFrameworkCore;
using TCS_Cliente.Data;
using Microsoft.OpenApi.Models;

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
                webBuilder.ConfigureServices((context, services) =>
                {
                    // Registrando o serviço UserService
                    services.AddScoped<IUserService, UserService>();

                    // Adicionando o contexto do banco de dados
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=app.db"));

                    // Adicionando o Swagger
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCS Cliente API", Version = "v1" });
                    });

                    services.AddControllers();
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();

                    // Habilitando o Swagger
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TCS Cliente API v1");
                        c.RoutePrefix = "swagger"; // Mudei para 'swagger'
                    });


                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            });
}
