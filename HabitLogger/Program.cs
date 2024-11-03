namespace HabitLogger
{
    class Program
    {
        static String[] menuOptions = new string[] {"c", "r", "u", "d", "0", "exit"};
        static void Main(string[] args)
        {
            bool exitProgram = false;
            string option;

            while (!exitProgram)
            {
                DisplayMenu();
                option = GetMenuInput();

                switch (option)
                {
                    case "c":
                        break;
                    case "r":
                        break;
                    case "u":
                        break;
                    case "d":
                        break;                        
                    case "0":
                    case "exit":
                        exitProgram = true;
                        break;
                    default:
                        exitProgram = true;
                        break;
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("*****    HABIT LOGGER    *****\n");
            Console.WriteLine("C - Create entry");
            Console.WriteLine("R - Read log");
            Console.WriteLine("U - Update entry");
            Console.WriteLine("D - Delete entry\n");
            Console.WriteLine("0 - Exit");
        }
        static string GetMenuInput()
        {
            String? input;
            do
            {
                Console.Write("\nType option C R U D or Exit: ");
                input = Console.ReadLine();
            } while (input is null || !menuOptions.Contains(input.ToLower()));
            return input.ToLower();
        }
    }
}
