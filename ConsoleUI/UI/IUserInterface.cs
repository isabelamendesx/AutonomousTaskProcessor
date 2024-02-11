using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI.UI
{
    public interface IUserInterface
    {
        Task HandleMenuOption(char option);
        void ShowMenu();
        void PrintRunningTasks();
    }
}
