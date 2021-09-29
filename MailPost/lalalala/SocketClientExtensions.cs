
namespace System
{


    // https://github.com/Zintom/SocketExtensions/tree/master/SocketExtensions
    // https://github.com/mono/mono/blob/main/mcs/class/System/System.Net.Sockets/SocketTaskExtensions.cs
    // https://github.com/Zintom/SocketExtensions/blob/master/SocketExtensions/BetterSocketExtensions.cs
    // https://github.com/Zintom/SocketExtensions/blob/master/SocketExtensions/TaskSocketExtensions.cs
    // https://github.com/mono/mono/blob/main/mcs/class/System/System.Net.Sockets/Socket.cs
    // https://github.com/microsoft/referencesource/blob/master/System/net/System/Net/Sockets/Socket.cs
    internal static class SocketClientExtensions
    {

        // SendAsync(bytes, SocketFlags.None, cancellationToken);


        private static void OnEndReceive(System.IAsyncResult ar)
        {
            var _tcs = (System.Threading.Tasks.TaskCompletionSource<int>)ar.AsyncState;
            var _socket = (System.Net.Sockets.Socket)_tcs.Task.AsyncState;

            try { _tcs.TrySetResult(_socket.EndReceive(ar)); }
            catch (System.Exception e) { _tcs.TrySetException(e); }
        }

        private static void OnEndSend(System.IAsyncResult ar)
        {
            var t = (System.Threading.Tasks.TaskCompletionSource<int>)ar.AsyncState;
            var s = (System.Net.Sockets.Socket)t.Task.AsyncState;

            try { t.TrySetResult(s.EndSend(ar)); }
            catch (System.Exception e) { t.TrySetException(e); }
        }



        /// <inheritdoc cref="Socket.Receive(byte[], int, int, SocketFlags)"/>
        public static System.Threading.Tasks.Task<int> ReceiveAsync(this System.Net.Sockets.Socket socket
            , byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<int>(socket);

            socket.BeginReceive(buffer, offset, size, socketFlags, OnEndReceive, tcs);

            return tcs.Task;
        }


        public static System.Threading.Tasks.Task<int> ReceiveAsync(
              this System.Net.Sockets.Socket socket,
              System.Collections.Generic.IList<System.ArraySegment<byte>> buffers,
              System.Net.Sockets.SocketFlags socketFlags)
        {
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (targetBuffers, flags, callback, state) => ((System.Net.Sockets.Socket)state).BeginReceive(targetBuffers, flags, callback, state),
                asyncResult => ((System.Net.Sockets.Socket)asyncResult.AsyncState).EndReceive(asyncResult),
                buffers,
                socketFlags,
                state: socket);
        }

        public static System.Threading.Tasks.Task<int> ReceiveAsync(
            this System.Net.Sockets.Socket socket,
            byte[] buffer,
            System.Net.Sockets.SocketFlags socketFlags)
        {
            return ReceiveAsync(socket, new System.Collections.Generic.List<System.ArraySegment<byte>>() { new System.ArraySegment<byte>(buffer) }, socketFlags);
        }


        public static System.Threading.Tasks.Task<int> ReceiveAsync(
            this System.Net.Sockets.Socket socket,
            byte[] buffer,
            System.Net.Sockets.SocketFlags socketFlags,
            System.Threading.CancellationToken token)
        {
            return ReceiveAsync(socket, buffer, socketFlags);
        }



        /// <inheritdoc cref="Socket.Send(byte[], int, int, SocketFlags)"/>
        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket, byte[] buffer
            , int offset, int size, System.Net.Sockets.SocketFlags socketFlags)
        {
            // We have to pass the Socket and TCS as state objects because we are using a static
            // anonymous function which cannot access locals or fields defined outside of it.
            var tcs = new System.Threading.Tasks.TaskCompletionSource<int>(socket);

            // We use static anonymous functions as we do not want to capture any outside variables, capturing variables
            // causes each captured variable to be allocated as a boxed object.
            socket.BeginSend(buffer, offset, size, socketFlags, OnEndSend, tcs);

            return tcs.Task;
        }



        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket
            , System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags)
        {
            return System.Threading.Tasks.Task<int>.Factory.FromAsync(
                (targetBuffer, flags, callback, state) => ((System.Net.Sockets.Socket)state).BeginSend(
                                                              targetBuffer.Array,
                                                              targetBuffer.Offset,
                                                              targetBuffer.Count,
                                                              flags,
                                                              callback,
                                                              state),
                asyncResult => ((System.Net.Sockets.Socket)asyncResult.AsyncState).EndSend(asyncResult),
                buffer,
                socketFlags,
                state: socket);
        }


        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket
            , byte[] buffer, System.Net.Sockets.SocketFlags socketFlags)
        {
            return SendAsync(socket, new System.ArraySegment<byte>(buffer), socketFlags);
        }


        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket
            , byte[] buffer, System.Net.Sockets.SocketFlags socketFlags, System.Threading.CancellationToken cancellationToken)
        {
            return socket.SendAsync(buffer, socketFlags);
        }


    }


}
