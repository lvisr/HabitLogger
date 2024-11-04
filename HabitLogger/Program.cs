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

            IntPtr db;
            string databasePath = "habit.db";

            // Open SQLite connection
            int result = SQLiteInterop.sqlite3_open(databasePath, out db);
            SQLiteInterop.CheckSQLiteError(result, db);

            // Create a table
            string createTableSQL = "CREATE TABLE IF NOT EXISTS CaffeineTracking (id INTEGER PRIMARY KEY, date TEXT, expresso_number INTEGER);";
            ExecuteNonQuery(db, createTableSQL);

            // Insert data
            string insertSQL = "INSERT INTO CaffeineTracking (date, expresso_number) VALUES ('2024-11-03', '2');";
            ExecuteNonQuery(db, insertSQL);

            // Close SQLite connection
            SQLiteInterop.sqlite3_close(db);

            Console.WriteLine("Database operations completed successfully.");
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
    }
}
