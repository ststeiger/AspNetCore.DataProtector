
namespace AmhMailServer
{
    internal class ColorConsole
    {

        public static object LOCK = new object();


        public static void Log(string output, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            System.ConsoleColor bgColor = System.Console.BackgroundColor;
            System.ConsoleColor fgColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = fg;
            System.Console.BackgroundColor = bg;

            System.Console.Write(output);

            System.Console.BackgroundColor = bgColor;
            System.Console.ForegroundColor = fgColor;
        }

        public static void Log(string output)
        {
            Log(output, System.ConsoleColor.White, System.ConsoleColor.Green);
        }


        public static void LogError(string output)
        {
            Log(output, System.ConsoleColor.White, System.ConsoleColor.Red);
        }


        public static void LogWithLock(string output, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            lock (LOCK)
            {
                Log(output, fg, bg);
            }
        }


        public static void LogWithLock(string output)
        {
            LogWithLock(output, System.ConsoleColor.White, System.ConsoleColor.Green);
        }


        public static void LogLine(string output, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            System.ConsoleColor bgColor = System.Console.BackgroundColor;
            System.ConsoleColor fgColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = fg;
            System.Console.BackgroundColor = bg;


            // Finish the line with empty color
            System.Console.Write(output);
            System.Console.Write(new string(' ', System.Console.BufferWidth - System.Console.CursorLeft));

            System.Console.BackgroundColor = bgColor;
            System.Console.ForegroundColor = fgColor;

            System.Console.Write(System.Environment.NewLine);
        }


        public static void LogLine(string output)
        {
            LogLine(output, System.ConsoleColor.White, System.ConsoleColor.Green);
        }


        public static void LogErrorLine(string output)
        {
            LogLine(output, System.ConsoleColor.White, System.ConsoleColor.Red);
        }

        public static void LogErrorWithLock(System.Exception exInput)
        {
            System.Exception ex = exInput;

            lock (LOCK)
            {
                while(ex != null)
                {
                    ColorConsole.LogErrorLine("");
                    ColorConsole.LogErrorLine(ex.Message);
                    ColorConsole.LogErrorLine("");
                    ColorConsole.LogErrorLine(ex.StackTrace);
                    ColorConsole.LogErrorLine("");

                    ex = ex.InnerException;
                }                
            }

        }



        public static void LogLineWithLock(string output, System.ConsoleColor fg, System.ConsoleColor bg)
        {
            lock (LOCK)
            {
                LogLine(output, fg, bg);
            }
        }


        public static void LogLineWithLock(string output)
        {
            LogLineWithLock(output, System.ConsoleColor.White, System.ConsoleColor.Green);
        }



        public static void LogErrorLineWithLock(string output)
        {
            LogLineWithLock(output, System.ConsoleColor.White, System.ConsoleColor.Red);
        }


    }
}
