using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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


    public static class JsonExtensions
    {
        private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = 
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            }
        ;

        public static T FromJson<T>(this string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
            

        public static string ToJson<T>(this T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize<T>(obj, _jsonOptions);
        }
            
    }



    // https://github.com/Zintom/SocketExtensions/tree/master/SocketExtensions
    // https://github.com/mono/mono/blob/main/mcs/class/System/System.Net.Sockets/SocketTaskExtensions.cs
    // https://github.com/Zintom/SocketExtensions/blob/master/SocketExtensions/BetterSocketExtensions.cs
    // https://github.com/Zintom/SocketExtensions/blob/master/SocketExtensions/TaskSocketExtensions.cs
    // https://github.com/mono/mono/blob/main/mcs/class/System/System.Net.Sockets/Socket.cs
    // https://github.com/microsoft/referencesource/blob/master/System/net/System/Net/Sockets/Socket.cs
    internal static class SocketClientExtensions
    {

        // SendAsync(bytes, SocketFlags.None, cancellationToken);


        private static void OnEndReceive(IAsyncResult ar)
        {
            var _tcs = (TaskCompletionSource<int>)ar.AsyncState;
            var _socket = (Socket)_tcs.Task.AsyncState;

            try { _tcs.TrySetResult(_socket.EndReceive(ar)); }
            catch (Exception e) { _tcs.TrySetException(e); }
        }

        private static void OnEndSend(IAsyncResult ar)
        {
            var t = (TaskCompletionSource<int>)ar.AsyncState;
            var s = (Socket)t.Task.AsyncState;

            try { t.TrySetResult(s.EndSend(ar)); }
            catch (Exception e) { t.TrySetException(e); }
        }



        /// <inheritdoc cref="Socket.Receive(byte[], int, int, SocketFlags)"/>
        public static Task<int> ReceiveAsync(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            var tcs = new TaskCompletionSource<int>(socket);

            socket.BeginReceive(buffer, offset, size, socketFlags, OnEndReceive, tcs);

            return tcs.Task;
        }


        public static Task<int> ReceiveAsync(
              this Socket socket,
              IList<ArraySegment<byte>> buffers,
              SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(
                (targetBuffers, flags, callback, state) => ((Socket)state).BeginReceive(targetBuffers, flags, callback, state),
                asyncResult => ((Socket)asyncResult.AsyncState).EndReceive(asyncResult),
                buffers,
                socketFlags,
                state: socket);
        }

        public static Task<int> ReceiveAsync(
            this Socket socket,
            byte[] buffer,
            SocketFlags socketFlags)
        {
            return ReceiveAsync(socket, new System.Collections.Generic.List<ArraySegment<byte>>() { new ArraySegment<byte>(buffer) }, socketFlags);
        }


        public static Task<int> ReceiveAsync(
            this Socket socket,
            byte[] buffer,
            SocketFlags socketFlags, 
            CancellationToken token)
        {
            return ReceiveAsync(socket, buffer, socketFlags);
        }



        /// <inheritdoc cref="Socket.Send(byte[], int, int, SocketFlags)"/>
        public static Task<int> SendAsync(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            // We have to pass the Socket and TCS as state objects because we are using a static
            // anonymous function which cannot access locals or fields defined outside of it.
            var tcs = new TaskCompletionSource<int>(socket);

            // We use static anonymous functions as we do not want to capture any outside variables, capturing variables
            // causes each captured variable to be allocated as a boxed object.
            socket.BeginSend(buffer, offset, size, socketFlags, OnEndSend, tcs);

            return tcs.Task;
        }



        public static Task<int> SendAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(
                (targetBuffer, flags, callback, state) => ((Socket)state).BeginSend(
                                                              targetBuffer.Array,
                                                              targetBuffer.Offset,
                                                              targetBuffer.Count,
                                                              flags,
                                                              callback,
                                                              state),
                asyncResult => ((Socket)asyncResult.AsyncState).EndSend(asyncResult),
                buffer,
                socketFlags,
                state: socket);
        }


        public static Task<int> SendAsync(this Socket socket, byte[] buffer, SocketFlags socketFlags)
        {
            return SendAsync(socket, new ArraySegment<byte>(buffer), socketFlags);
        }


        public static Task<int> SendAsync(this Socket socket, byte[] buffer, SocketFlags socketFlags, CancellationToken cancellationToken)
        {
            return socket.SendAsync(buffer, socketFlags);
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


    public class SomeDispose
        : System.IDisposable
    {
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~SomeDispose()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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


    class ExceptionExtensions
    {

        public static void Log(System.Exception ex, string where, string assemb, string file, int? line)
        {
            System.Exception ex2 = ex;

            while (ex2 != null)
            {
                System.Console.WriteLine(ex2.Message);
                System.Console.WriteLine(Environment.NewLine);
                System.Console.WriteLine(ex2.StackTrace);
                System.Console.WriteLine(Environment.NewLine);
                ex2 = ex2.InnerException;
            }
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
