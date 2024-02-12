
namespace ConsoleUI.Resources;

public static class Utilities
{
    public static int ReadInteger(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int number;
        bool validNumber = false;

        do
        {
            Console.Write($"{prompt}: ");
            string input = Console.ReadLine()!;

            if (int.TryParse(input, out number) && number >= minValue && number <= maxValue)
            {
                validNumber = true;
            }
            else
            {
                Console.WriteLine($"You did not enter a valid number within the range of {minValue} to {maxValue}. Please try again.");
            }
        } while (!validNumber);

        return number;
    }
}
