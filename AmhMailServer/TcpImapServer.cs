
namespace AmhMailServer
{


    public class ImapTcpSession
        : NetCoreServer.TcpSession
    {



        public ImapTcpSession(NetCoreServer.TcpServer server)
            : base(server)
        { }



        protected void Write(string message)
        {
            lock (ColorConsole.LOCK)
            {
                ColorConsole.Log("[SERVER]: Sending message \"");
                ColorConsole.Log(message);
                ColorConsole.LogLine("\\r\\n\"");
            }

            SendAsync(System.Text.Encoding.ASCII.GetBytes(message + "\r\n"));
        }


        protected override void OnConnected()
        {
            ColorConsole.LogLineWithLock($"[SERVER]: TCP session with Id {Id} connected!", System.ConsoleColor.Black, System.ConsoleColor.Yellow);
            

            // Send invite message
            // string message = "Hello from Syslog TCP session ! Please send a message or '!' to disconnect the client!"; SendAsync(message);
            Write("220 localhost -- Fake proxy server");
        } // End Sub OnConnected 


        protected override void OnDisconnected()
        {
            ColorConsole.LogLineWithLock($"[SERVER]: TCP session with Id {Id} disconnected!", System.ConsoleColor.Black, System.ConsoleColor.Yellow);
        } // End Sub OnDisconnected 




        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            try
            {
                string message = string.Empty;
                string printMessage = string.Empty;

                // throw new System.Exception("FOOBAR - Fucked up beyond all repair !");


                try
                {
                    message = System.Text.Encoding.ASCII.GetString(buffer, (int)offset, (int)size);
                    printMessage = message.Replace("\r", "\\r").Replace("\n", "\\n");

                    lock (ColorConsole.LOCK)
                    {
                        ColorConsole.Log("[SERVER]: Received message \"");
                        ColorConsole.Log(printMessage);
                        ColorConsole.LogLine("\".");
                    }
                }
                catch (System.Exception e)
                {
                    //a socket error has occured
                    System.Console.WriteLine(e.Message);
                }

                if (message.Length > 0)
                {
                    if (message.IndexOf("QUIT") != -1 && !message.StartsWith("QUIT"))
                    {
                        ColorConsole.LogErrorLineWithLock("[SERVER]: Missed QUIT SIGNAL - Message: \"" + printMessage + "\".");
                    }

                }

            }
            catch (System.Exception ex)
            {
                ColorConsole.LogErrorWithLock(ex);
            }

            // System.Console.WriteLine("exiting message loop");

            // Multicast message to all connected sessions
            // Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            // if (message == "!")
            // Disconnect();

        } // End Sub OnReceived 


        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            ColorConsole.LogErrorLineWithLock($"[SERVER]: TCP session caught an error with code {error}");
        } // End Sub OnError 


    } // End Class SmtpTcpSession 


    // https://stackoverflow.com/questions/16809214/is-smtp-based-on-tcp-or-udp
    public class TcpImapServer
        : NetCoreServer.TcpServer
    {

        public TcpImapServer(System.Net.IPAddress address, int port)
            : base(address, port)
        { }


        protected override NetCoreServer.TcpSession CreateSession()
        {
            return new ImapTcpSession(this);
        } // End Function CreateSession 


        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            ColorConsole.LogErrorLineWithLock($"[SERVER]: TCP server caught an error with code {error}");
        } // End Sub OnError 


        public static void Test()
        {
            // TCP server port
            int port = 143;
            // Port 143 – this is the default IMAP non-encrypted port;
            // Port 993 – this is the port you need to use if you want to connect using IMAP securely.

            System.Console.WriteLine($"TCP server port: {port}");

            System.Console.WriteLine();

            // Create a new TCP Syslog server
            TcpImapServer server =
                new TcpImapServer(System.Net.IPAddress.Any, port);

            // Start the server
            System.Console.WriteLine("Server starting...");
            server.Start();
            System.Console.WriteLine("Done!");

            System.Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            while (true)
            {
                System.ConsoleKeyInfo line = System.Console.ReadKey();

                // Restart the server
                if (line.KeyChar == '!')
                {
                    System.Console.WriteLine("Server restarting...");
                    server.Restart();
                    System.Console.WriteLine("Done!");
                    continue;
                } // End if (line == "!") 
                else
                    break;

                // Multicast admin message to all sessions
                // server.Multicast(line);
            } // Next 

            // Stop the server
            System.Console.WriteLine("Server stopping...");
            server.Stop();
            System.Console.WriteLine("Done!");
        } // End Sub Test  


    } // End Class TcpSmtpServer 


} // End Namespace SyslogServer 
