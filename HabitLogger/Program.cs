namespace HabitLogger
{
    class Program
    {
        static String[] menuOptions = new string[] {"c", "r", "u", "d", "0", "exit"};
        static void Main(string[] args)
        {
            bool exitProgram = false;
            string option;

            IntPtr db;
            string databasePath = "habit.db";

            // Open SQLite connection
            int result = SQLiteInterop.sqlite3_open(databasePath, out db);
            SQLiteInterop.CheckSQLiteError(result, db);

            // Create a table
            string createTableSQL = "CREATE TABLE IF NOT EXISTS CaffeineTracking (id INTEGER PRIMARY KEY, date TEXT, expresso_number INTEGER);";
            ExecuteNonQuery(db, createTableSQL);
            while (!exitProgram)
            {
                DisplayMenu();
                option = GetMenuInput();

                switch (option)
                {
                    case "c":
                        string date = GetDateInput();
                        string message ="\nType number of coffees consumed: ";
                        int expresso_number = GetIntInput(message);
                        InsertRecord(db, date, expresso_number);
                        PressEnterToContinue();
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
            // Close SQLite connection
            SQLiteInterop.sqlite3_close(db);
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("*****    CAFFEINE TRACKER    *****\n");
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
        static string GetDateInput()
        {
            String? input;
            bool validDate = false;
            ;
            do
            {
                Console.Write("\nType desired date with format YYYY-MM-DD:  ");
                input = Console.ReadLine();
                
                if (input is not null && input.Split('-').Length == 3)
                {
                    string[] dateParts = input.Split('-');

                    int year; int month; int day;

                    if (int.TryParse(dateParts[0], out year) && year > 1900 && year <= 2024
                        && int.TryParse(dateParts[1], out month) && month > 0 && month <= 12
                        && int.TryParse(dateParts[2], out day) && day > 0 && day <= 31)
                        validDate = true;
                }
            } while (input is null || input.Split('-').Length != 3 || !validDate);

            return input;
        }
        static int GetIntInput(string message)
        {
            String? input;
            bool success = false;
            int result;
            do
            {
                Console.Write(message);
                input = Console.ReadLine();
                if (input is not null && int.TryParse(input, out result) && result > 0)
                    return result;

            } while (!success);
            return 0;
        }
        static void PressEnterToContinue()
        {   
            Console.WriteLine("\nPress [Enter] to continue.");
            Console.ReadLine();
        }

        static void ExecuteNonQuery(IntPtr db, string sql)
        {
            IntPtr stmt;
            int result = SQLiteInterop.sqlite3_prepare_v2(db, sql, sql.Length, out stmt, IntPtr.Zero);
            SQLiteInterop.CheckSQLiteError(result, db);

            result = SQLiteInterop.sqlite3_step(stmt);
            if (result != SQLiteInterop.SQLITE_DONE)
            {
                SQLiteInterop.CheckSQLiteError(result, db);
            }

            SQLiteInterop.sqlite3_finalize(stmt);
        }
        static void InsertRecord(IntPtr db, string date, int expresso_number)
        {
            string insertSQL = $"INSERT INTO CaffeineTracking (date, expresso_number) VALUES ('{date}', '{expresso_number}');";
            IntPtr stmt;
            int result = SQLiteInterop.sqlite3_prepare_v2(db, insertSQL, insertSQL.Length, out stmt, IntPtr.Zero);
            SQLiteInterop.CheckSQLiteError(result, db);

            // Bind the name parameter
            SQLiteInterop.sqlite3_bind_text(stmt, 1, date, -1, IntPtr.Zero);
            SQLiteInterop.sqlite3_bind_int(stmt, 2, expresso_number);

            result = SQLiteInterop.sqlite3_step(stmt);
            if (result != SQLiteInterop.SQLITE_DONE)
            {
                SQLiteInterop.CheckSQLiteError(result, db);
            }

            SQLiteInterop.sqlite3_finalize(stmt);
            Console.WriteLine("\nInserted on Database:");
            Console.WriteLine($"On {date} consumed {expresso_number} coffees.");
        }
    }
}
