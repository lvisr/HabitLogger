using System.Runtime.InteropServices;

namespace HabitLogger
{
    public class CoffeeLogEntry
    {
        public int id { get; set; }
        public string? date { get; set; }
        public int drinksNumber { get; set; }
    }
    class Program
    {
        static String[] menuOptions = new string[] {"c", "r", "u", "d", "0", "exit"};
        static List<CoffeeLogEntry> coffeeLog = new List<CoffeeLogEntry>();
        
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
            string createTableSQL = "CREATE TABLE IF NOT EXISTS CaffeineTracking (id INTEGER PRIMARY KEY, date TEXT, drinksNumber INTEGER);";
            ExecuteNonQuery(db, createTableSQL);
            while (!exitProgram)
            {
                DisplayMenu();
                option = GetMenuInput();

                switch (option)
                {
                    case "c":
                        string date = GetDateInput();
                        string message ="\nType number of coffee drinks consumed: ";
                        int drinksNumber = GetIntInput(message);
                        InsertRecord(db, date, drinksNumber);
                        PressEnterToContinue();
                        break;
                    case "r":
                        Console.Clear();
                        ReadRecords(db);
                        PressEnterToContinue();
                        break;
                    case "u":
                        Console.Clear();                       
                        string messageId ="\nWhich record would you like to update? Type id: ";
                        int idToUpdate = GetIntInput(messageId);
                        string newDate = GetDateInput();
                        string messageNewDrinks ="\nType new number of coffee drinks consumed: ";
                        int newDrinksNumber = GetIntInput(messageNewDrinks);
                        UpdateRecord(db, idToUpdate, newDate, newDrinksNumber);
                        PressEnterToContinue();
                        break;
                    case "d":
                        PressEnterToContinue();
                        break;                        
                    case "0":
                    case "exit":
                        exitProgram = true;
                        break;
                    default:
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
        static void InsertRecord(IntPtr db, string date, int drinksNumber)
        {
            string insertSQL = $"INSERT INTO CaffeineTracking (date, drinksNumber) VALUES ('{date}', '{drinksNumber}');";
            IntPtr stmt;
            int result = SQLiteInterop.sqlite3_prepare_v2(db, insertSQL, insertSQL.Length, out stmt, IntPtr.Zero);
            SQLiteInterop.CheckSQLiteError(result, db);

            // Bind the name parameter
            SQLiteInterop.sqlite3_bind_text(stmt, 1, date, -1, IntPtr.Zero);
            SQLiteInterop.sqlite3_bind_int(stmt, 2, drinksNumber);

            result = SQLiteInterop.sqlite3_step(stmt);
            if (result != SQLiteInterop.SQLITE_DONE)
            {
                SQLiteInterop.CheckSQLiteError(result, db);
            }

            SQLiteInterop.sqlite3_finalize(stmt);
            Console.WriteLine("\nInserted on Database:");
            Console.WriteLine($"On {date} consumed {drinksNumber} coffee drinks.");
        }
        static void ReadRecords(IntPtr db)
        {
            List<CoffeeLogEntry> auxLog = new List<CoffeeLogEntry>();
            string selectSQL = "SELECT id, date, drinksNumber FROM CaffeineTracking;";
            IntPtr stmt;
            int result = SQLiteInterop.sqlite3_prepare_v2(db, selectSQL, selectSQL.Length, out stmt, IntPtr.Zero);
            SQLiteInterop.CheckSQLiteError(result, db);

            Console.WriteLine("ID \tDate\t\tCoffee Drinks");
            while ((result = SQLiteInterop.sqlite3_step(stmt)) == SQLiteInterop.SQLITE_ROW)
            {
                int id = SQLiteInterop.sqlite3_column_int(stmt, 0);
                string? date = Marshal.PtrToStringAnsi(SQLiteInterop.sqlite3_column_text(stmt, 1));
                int drinksNumber = SQLiteInterop.sqlite3_column_int(stmt, 2);

                auxLog.Add(new CoffeeLogEntry{ id = id, date = date, drinksNumber = drinksNumber});

                //Console.WriteLine($"{id}\t{date}\t\t{drinksNumber}");
            }
            coffeeLog = auxLog;
            foreach (var logEntry in coffeeLog)
            {
                Console.WriteLine($"{logEntry.id}\t{logEntry.date}\t\t{logEntry.drinksNumber}");
            }

            SQLiteInterop.sqlite3_finalize(stmt);
        }
        static void UpdateRecord(IntPtr db, int id, string newDate, int newDrinksNumber)
        {
            string updateSQL = "UPDATE CaffeineTracking SET date = @date, drinksNumber = @drinksNumber WHERE id = @id;";
            IntPtr stmt;
            int result = SQLiteInterop.sqlite3_prepare_v2(db, updateSQL, updateSQL.Length, out stmt, IntPtr.Zero);
            SQLiteInterop.CheckSQLiteError(result, db);

            // Bind parameters
            SQLiteInterop.sqlite3_bind_text(stmt, 1, newDate, -1, IntPtr.Zero);
            SQLiteInterop.sqlite3_bind_int(stmt, 2, newDrinksNumber);
            SQLiteInterop.sqlite3_bind_int(stmt, 3, id);

            result = SQLiteInterop.sqlite3_step(stmt);
            if (result != SQLiteInterop.SQLITE_DONE)
            {
                SQLiteInterop.CheckSQLiteError(result, db);
            }

            SQLiteInterop.sqlite3_finalize(stmt);
            Console.WriteLine($"Updated record with ID {id} to:\nOn {newDate} consumed {newDrinksNumber} coffee drinks.");
        }

    }
}
