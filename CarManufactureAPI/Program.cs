using CarManufactureAPI.Filters;
using CarManufactureAPI.Repositories;
using CarManufactureAPI.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CarManufactureAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de servicios de negocio y repositorios
            builder.Services.AddSingleton<ISalesRepository, InMemorySalesRepository>();
            builder.Services.AddScoped<ISalesService, SalesService>();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Car Manufacture API",
                    Description = "API REST para gestión de ventas de fábrica de automóviles. ",
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Swagger disponible en todos los ambientes para facilitar testing
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Manufacture API v1");
                //options.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
