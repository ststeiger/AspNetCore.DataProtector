
namespace AmhMailServer
{


    // https://www.codeproject.com/Tips/286952/create-a-simple-smtp-server-in-csharp
    class MyClient
    {


        public static void Test(System.Net.Mail.MailMessage message)
        {
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("localhost");
            smtp.Send(message);//Handles all messages in the protocol
            smtp.Dispose();//sends a Quit message
        }


        public static void StartServer()
        {
            System.Collections.Generic.List<SMTPServer> servers = new System.Collections.Generic.List<SMTPServer>();

            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 25);
            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(endPoint);
            listener.Start();

            while (true)
            {
                System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
                SMTPServer handler = new SMTPServer();
                servers.Add(handler);
                handler.Init(client);
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(handler.Run));
                thread.Start();
            }
        }


    }


}
