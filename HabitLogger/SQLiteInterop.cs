using System.Runtime.InteropServices;

namespace HabitLogger
{
    public class SQLiteInterop
    {
        private const string SQLITE_DLL = "libsqlite3.so";

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_open(string filename, out IntPtr db);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_close(IntPtr db);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_prepare_v2(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr tail);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_step(IntPtr stmt);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_finalize(IntPtr stmt);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_errmsg", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_errmsg(IntPtr db);

        // binding functions to prevent SQL injection
        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_text(IntPtr stmt, int index, string value, int length, IntPtr destructor);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_int(IntPtr stmt, int index, int value);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_column_text(IntPtr stmt, int column);

        [DllImport(SQLITE_DLL, EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_int(IntPtr stmt, int column);
        
        public const int SQLITE_OK = 0;
        public const int SQLITE_ROW = 100;
        public const int SQLITE_DONE = 101;

        public static void CheckSQLiteError(int resultCode, IntPtr db)
        {
            if (resultCode != SQLITE_OK)
            {
                IntPtr errMsg = sqlite3_errmsg(db);
                throw new Exception(Marshal.PtrToStringAnsi(errMsg));
            }
        }
    }
}