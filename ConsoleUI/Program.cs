using ConsoleUI.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorAutonomo.Data;
using ProcessadorAutonomo.Entities;
using ProcessadorAutonomo.Repositories;
using ProcessadorAutonomo.Services;
using Repositories;

namespace ConsoleUI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            try
            {
                var serviceprovider = ConfigServiceProvider();
                var processExecutor = serviceprovider.GetService<IProcessExecutor>();
                var processManager = serviceprovider.GetService<IProcessManager>();
                var UI = serviceprovider.GetService<IUserInterface>()!;

                var runningProcesses = processExecutor.ExecuteProcessInParallel();

                while (!runningProcesses.IsCompleted)
                {
                    Console.Clear();
                    UI.PrintRunningTasks();
                    UI.ShowMenu(); ;

                    Task<char> readInput = Task.Run(() =>
                    {
                        if (Console.KeyAvailable)
                        {
                            return Console.ReadKey(intercept: true).KeyChar;
                        }
                        return '\0';
                    });

                    var option = readInput.Result;
                    await UI.HandleMenuOption(option);
                }

                await runningProcesses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }


        static IServiceProvider ConfigServiceProvider()
        {
            var services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();


            services.AddDbContext<ProcessContext>();


            services.AddSingleton(configuration);
            services.AddSingleton<IUserInterface, UserInterface>();
            services.AddScoped<IRepository<Process>, SqliteRepository>();
            services.AddScoped<IProcessExecutor, ProcessExecutor>();
            services.AddScoped<IProcessManager, ProcessManager>();

            return services.BuildServiceProvider();
        }

    }
}

