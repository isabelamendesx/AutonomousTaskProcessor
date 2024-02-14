
using AutonomousTaskProcessor.Data;
using AutonomousTaskProcessor.Entities;
using AutonomousTaskProcessor.Repositories;
using AutonomousTaskProcessor.Services;
using Repositories;
using System;
using TaskProcessorAPI.Configurations;

namespace TaskProcessorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Acho que isso aqui como singleton ta muito errado, tirar dúvida dps
            builder.Services.AddDbContext<ProcessContext>(ServiceLifetime.Singleton);

            builder.Services.AddSingleton<IProcessRepository, SqliteRepository>();
            builder.Services.AddSingleton<IProcessExecutor, ProcessExecutor>();
            builder.Services.AddSingleton<IProcessManager, ProcessManager>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}