using ConsoleUI.Resources;
using AutonomousTaskProcessor.Services;
using AutonomousTaskProcessor.Entities;


namespace ConsoleUI.UI;

public class UserInterface : IUserInterface
{
    private readonly IProcessManager _processManager;
    private readonly IProcessExecutor _processExecutor;
    private Dictionary<StatusProcess, ConsoleColor> statusColorMap = new Dictionary<StatusProcess, ConsoleColor>()
                {
                     { StatusProcess.Scheduled, ConsoleColor.Magenta },
                     { StatusProcess.InProgress, ConsoleColor.Yellow },
                     { StatusProcess.Paused, ConsoleColor.DarkYellow },
                     { StatusProcess.Canceled, ConsoleColor.Red },
                     { StatusProcess.Completed, ConsoleColor.Green },
                     { StatusProcess.Created, ConsoleColor.White }
                };


    public UserInterface(IProcessManager processManager, IProcessExecutor processExecutor)
    {
        _processManager = processManager;
        _processExecutor = processExecutor;
    }

    public void ShowActiveProcesses()
    {
        var activeProcesses = _processManager.ListActiveProcesses().GetAwaiter().GetResult();

        foreach (var process in activeProcesses)
        {
            string statusColor;
            switch (process.Status)
            {
                case StatusProcess.Scheduled:
                    statusColor = ConsoleColor.Yellow.ToString();
                    break;
                case StatusProcess.InProgress:
                    statusColor = ConsoleColor.Green.ToString();
                    break;
                case StatusProcess.Paused:
                    statusColor = ConsoleColor.DarkYellow.ToString();
                    break;
                default:
                    statusColor = ConsoleColor.White.ToString();
                    break;
            }

            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), statusColor);
            Console.WriteLine(process);
            Console.ForegroundColor = originalColor;
        }
    }

    public async Task HandleMenuOption(char option)
    {
        switch (option)
        {
            case '1':
                Console.WriteLine("Creating a new process...");
                await Task.Delay(1000);
                await _processManager.Create();
                //ShowActiveProcesses();
                PrintProcesses();
                await Task.Delay(5000);
                break;

            case '2':
                Console.WriteLine("Loading active processes...");
                await Task.Delay(1000);
                PrintProcesses(); ;
                await Task.Delay(5000);
                break;

            case '3':
                Console.WriteLine("Loading inactive processes...");
                await Task.Delay(1000);
                PrintProcesses(false);
                await Task.Delay(5000);
                break;

            case '4':
                Console.WriteLine("Are you sure you want to exit and stop all processes? Press 'Y' to continue or any other key to cancel.");
                string confirmation = Console.ReadLine()!.ToUpper();

                if (confirmation.Equals("Y"))
                {
                    await _processExecutor.Close();
                    Console.WriteLine("All processes canceled. Exiting..");
                }
                else
                {
                    Console.WriteLine("Action canceled. No processes were stopped.");
                }

                await Task.Delay(1000);
                break;

            case '5':
                PrintProcesses();
                int processId = Utilities.ReadInteger("Enter process ID to cancel", 1);
                var process = _processManager.Check(processId).Result ?? null;

                if(process != null)
                {
                    await CancelProcess(process);
                    break;
                }

                Console.WriteLine("Process not found. Action canceled.");
                break;
            case '\0':

                await Task.Delay(2000);
                break;

            default:
                Console.WriteLine("Invalid Option :(");
                break;

        }
    }

    public void ShowMenu()
    {
            Console.Title = "Autonomous Task Processor";
            Console.WriteLine("╔═════════════════════════════╗");
            Console.WriteLine("║            MENU             ║");
            Console.WriteLine("╠═════════════════════════════╣");
            Console.WriteLine("║ 1. Create                   ║");
            Console.WriteLine("║ 2. List Active Processes    ║");
            Console.WriteLine("║ 3. List Inactive Processes  ║");
            Console.WriteLine("║ 4. Stop all Processes       ║");
            Console.WriteLine("║ 5. Cancel Process           ║");
            Console.WriteLine("╚═════════════════════════════╝");
    }

    public void PrintRunningTasks()
    {
        var runningProcesses = GetActiveProcesses().Where(p => p.Status.Equals(StatusProcess.InProgress));

        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║               Running Processes                ║");
        Console.WriteLine("╠════════════════════════════════════════════════╣");
        Console.WriteLine("║         Name         |  Completed Subprocess   ║");
        Console.WriteLine("╠════════════════════════════════════════════════╣");

        foreach (var process in runningProcesses)
        {
            var concludedSubProcesses = process.SubProcesses.Where(sp => sp.isConcluded).Count();
            var totalSubProcesses = process.SubProcesses.Count();

            PrintColoredText($"║  \u001b[5mProcess {process.Id,-12}\u001b[0m |   {concludedSubProcesses.ToString().PadLeft(2)}/{totalSubProcesses,-17} ║", ConsoleColor.Green);
            PrintProgressBar(concludedSubProcesses, totalSubProcesses);
            Console.WriteLine("╠════════════════════════════════════════════════╣");
        }

        Console.WriteLine("╚════════════════════════════════════════════════╝");
    }

    public async Task RestartOrResume()
    {
        Console.WriteLine("Press 'R' to restart processing from 0 or any other key to resume the last saved processing");

        if (Console.ReadLine()!.ToUpper().Equals("R"))
        {
            Console.WriteLine("Restarting application...");
            await Task.Delay(1000);
            await _processExecutor!.Restart();
        }

        Console.WriteLine("Starting from the last saved processing...");
    }

    private void PrintProgressBar(int completed, int total)
    {
        Console.Write("║ Progress: ");
        int maxBars = 35;
        int completedBars = (int)Math.Round((double)completed / total * maxBars);
        int remainingBars = maxBars - completedBars;
        string progressBar = new string('█', completedBars) + new string('░', remainingBars);
        Console.WriteLine($"{progressBar,-5}  ║");
    }

    private void PrintColoredText(string output, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(output);
        Console.ResetColor();
    }

    private async Task CancelProcess(Process process)
    {
        Console.WriteLine($"Canceling {process.Name} process...");

        if (process.Status.Equals(StatusProcess.InProgress))
        {
            await _processExecutor.CancelProcess(process.Id);
        }
        else
        {
            await _processManager.Cancel(process.Id);
        }

        Console.WriteLine($"{process.Name} canceled.");
        await Task.Delay(1500);
    }

    private void PrintProcesses(bool showActive = true)
    {
        var processes = showActive ? GetActiveProcesses() : GetInactiveProcesses();

        foreach(var process in processes)
        {
            var consoleColor = statusColorMap.FirstOrDefault(kvp => kvp.Key.Equals(process.Status)).Value;
            PrintColoredText(process.ToString()!, consoleColor);
        }    
    } 

    private IEnumerable<Process> GetActiveProcesses()
    {
        return _processManager.ListActiveProcesses().GetAwaiter().GetResult();
    }

    private IEnumerable<Process> GetInactiveProcesses()
    {
        return _processManager.ListInactiveProcesses().GetAwaiter().GetResult();
    }


}
