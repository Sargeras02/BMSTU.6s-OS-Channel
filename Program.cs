using Microsoft.OpenApi.Models;
using WebApi_KR.Helpers;

namespace WebApi_KR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HealHelper.PrintHealMeta();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "KR ST Channel Level",
                        Version = "v1"
                    }
                 );

                var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi-KR.xml");
                c.IncludeXmlComments(filePath);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}