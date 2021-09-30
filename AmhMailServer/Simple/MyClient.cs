
namespace AmhMailServer
{


    // https://www.codeproject.com/Tips/286952/create-a-simple-smtp-server-in-csharp
    public class MyClient
    {


        public static void Test(System.Net.Mail.MailMessage message)
        {
            lock (ColorConsole.LOCK)
            {
                System.Console.WriteLine("CLIENT: connecting");
            }
            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("localhost"))
            {
                lock (ColorConsole.LOCK)
                {
                    System.Console.WriteLine("CLIENT: connected");
                }

                // smtp.Timeout = 100000; // default
                smtp.Timeout = 5000;

                // smtp.EnableSsl = true;
                // smtp.UseDefaultCredentials = false;
                // smtp.Credentials = new System.Net.NetworkCredential("mais.sangue@hotmail.com", "M@is$angue");


                lock (ColorConsole.LOCK)
                {
                    System.Console.WriteLine("CLIENT: sending message");
                }

                try
                {
                    smtp.Send(message);

                    lock (ColorConsole.LOCK)
                    {
                        System.Console.WriteLine("CLIENT: Message successfully sent.");
                    }
                }
                catch (System.Exception ex)
                {

                    lock (ColorConsole.LOCK)
                    {
                        System.Console.WriteLine("CLIENT: Message could not be sent.\r\nReason: " + ex.Message + "\r\n" + ex.StackTrace);
                    }
                }

                lock (ColorConsole.LOCK)
                {
                    System.Console.WriteLine("CLIENT: disconnecting");
                }
            } // Dispose() sends a Quit message 

            lock (ColorConsole.LOCK)
            {
                System.Console.WriteLine("CLIENT: disconnected");
                System.Console.WriteLine("CLIENT: Test finished !");
            }
        } // End Sub Test 


        private static string ReverseGraphemeClusters(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length == 1)
                return s;

            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            System.Globalization.TextElementEnumerator enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                ls.Add((string)enumerator.Current);
            } // Whend 

            ls.Reverse();

            return string.Join("", ls.ToArray());
        } // End Sub Test 


        public static void Test()
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

            string from = ReverseGraphemeClusters("hc.tnemeganam-roc}ta{ksedecivres").Replace("{at}", "@");
            string to = ReverseGraphemeClusters("hc.tnemeganam-roc]ta[regiets").Replace("[at]", "@");
            // System.Console.WriteLine("{0} => {1}", from, to);


            msg.From = new System.Net.Mail.MailAddress(from);
            msg.To.Add( new System.Net.Mail.MailAddress(to) );

            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Subject = "Hello there";

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.Body = "This is a Test ! äöü ÄÖPÜ & %LOL.";
            
            Test(msg);
        } // End Sub Test 


        private static bool FOREVER = true;
        public static System.Net.Sockets.TcpListener listener;


        public static void StopServer()
        {
            // https://stackoverflow.com/questions/19220957/tcplistener-how-to-stop-listening-while-awaiting-accepttcpclientasync
            FOREVER = false;
            listener.Stop();
        } // End Sub StopServer 


        public static void StartServer()
        {
            System.Collections.Generic.List<SMTPServer> servers = new System.Collections.Generic.List<SMTPServer>();

            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 25);
            listener = new System.Net.Sockets.TcpListener(endPoint);
            listener.Start();

            while (FOREVER)
            {
                try
                {
                    System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
                    SMTPServer handler = new SMTPServer(client);
                    servers.Add(handler);
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(handler.Run));
                    thread.Start();
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    if(FOREVER) // Not a planned server shutdown 
                    {
                        System.Console.WriteLine(System.Environment.NewLine);
                        System.Console.WriteLine(se.Message);
                        System.Console.WriteLine(System.Environment.NewLine);
                        System.Console.WriteLine(se.StackTrace);
                        System.Console.WriteLine(System.Environment.NewLine);
                    }
                    else
                        System.Console.WriteLine("Server stopped.");
                } // End Catch System.Net.Sockets.SocketException 
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine(ex.StackTrace);
                    System.Console.WriteLine(System.Environment.NewLine);
                } // End Catch System.Exception 

            } // Whend 

        } // End Sub StartServer 


    } // End Class MyClient 


} // End Namespace AmhMailServer 
