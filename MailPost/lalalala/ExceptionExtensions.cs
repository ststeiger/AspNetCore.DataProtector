
namespace System
{

    class ExceptionExtensions
    {

        public static void Log(System.Exception ex, string where, string assemb, string file, int? line)
        {
            System.Exception ex2 = ex;

            while (ex2 != null)
            {
                System.Console.WriteLine(ex2.Message);
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(ex2.StackTrace);
                System.Console.WriteLine(System.Environment.NewLine);
                ex2 = ex2.InnerException;
            }
        }
    }


}
