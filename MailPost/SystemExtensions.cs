
namespace System
{

    
    public static class ConfigurationExtensions2
    {


        public static T TryGet<T>(Microsoft.Extensions.Configuration.IConfiguration config, string protocol
            , Action<Microsoft.Extensions.Configuration.BinderOptions> binder)
        {
            throw new System.NotImplementedException(nameof(TryGet));
            return default(T);
        }

    }
    

    public static class NetClientExtensions
    {

        public static void UseNetClient(Microsoft.Extensions.DependencyInjection.IServiceCollection service)
        { }
    }



    public static class NumberExtensions
    {

        public static long NewId(object obj, bool b)
        {
            throw new System.NotImplementedException(nameof(NewId));

            return 123;
        }
    }


    public class Globals
    {
        public static T GetService<T>()
        {
            throw new System.NotImplementedException(nameof(GetService));
            return default(T) ;
        }
    }


    class Utility
    {

        public static bool TryGetEncoding(string charset, ref System.Text.Encoding enc)
        {
            throw new System.NotImplementedException(nameof(TryGetEncoding));
            return true;
        }


    }


    

    public class FileLoggerExtensions
    {

        public static void Info(object logger, string format, params object[] args)
        {

        }


        public static void Warn(object logger, string format, params object[] args)
        {

        }


        public static void Debug(object logger, string format, params object[] args)
        {

        }

        public static void Error(object logger, string format, params object[] args)
        {

        }

        public static void Error(object logger, System.Exception ex, string format, params object[] args)
        {

        }


        public static void Fatal(object logger, string format, params object[] args)
        {

        }

        public static SomeDispose CreateScope(object logger, string id, bool b, bool bb)
        {
            return new SomeDispose();
        }

    }




    public static class SystemExtensions
    {

        public static bool IsEmpty(string s)
        {
            return s == string.Empty;
        }


        public static bool Contains(this string heystack, string needle, StringComparison comparison)
        {
            return heystack.IndexOf(needle, comparison) != -1;
        }


        public static T ConvertTo<T>(object value, string method, string file, int? val)
        {
            return (T)value;
        }


    }
}
