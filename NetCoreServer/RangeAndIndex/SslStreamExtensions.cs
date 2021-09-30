
namespace NetCoreServer
{


    internal static class SslStreamExtensions 
    {


        //public static Task AsTask(this object _obj)
        //      {
        //          object obj = _obj;
        //          object obj2;
        //          if (obj != null)
        //          {
        //              obj2 = obj as Task;
        //              if (obj2 == null)
        //              {
        //                  // return GetTaskForValueTaskSource(Unsafe.As<IValueTaskSource>(obj));
        //                  return Task.CompletedTask;
        //              }
        //          }
        //          else
        //          {
        //              obj2 = Task.CompletedTask;
        //          }
        //          return (Task)obj2;
        //      }


        private static System.Reflection.MethodInfo s_shutdownAsync = typeof(System.Net.Security.SslStream).GetMethod("ShutdownAsync",
             System.Reflection.BindingFlags.Instance
             | System.Reflection.BindingFlags.Public
             | System.Reflection.BindingFlags.NonPublic
         );



        internal static async System.Threading.Tasks.Task ShutdownAsync(this System.Net.Security.SslStream stream)
        {
            // ThrowIfExceptionalOrNotAuthenticatedOrShutdown();
            // ProtocolToken protocolToken = _context.CreateShutdownToken();
            // _shutdown = true;
            // return base.InnerStream.WriteAsync(protocolToken.Payload).AsTask();

            // https://github.com/dotnet/standard/issues/598
         
            if (s_shutdownAsync != null)
            {
                await ((System.Threading.Tasks.Task)(s_shutdownAsync.Invoke(stream, null)));
            }
            else
            {
                System.Console.WriteLine("!!!! Did not find ShutdownAsync. !!!! ");
                await System.Threading.Tasks.Task.CompletedTask;
            } 
        }


    }


}
