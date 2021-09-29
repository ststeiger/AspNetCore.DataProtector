
namespace AmhMailServer
{


    // https://www.codeproject.com/Tips/286952/create-a-simple-smtp-server-in-csharp
    class MyClient
    {


        public static void Test(System.Net.Mail.MailMessage message)
        {
            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("localhost"))
            { 
                // smtp.EnableSsl = true;
                // smtp.UseDefaultCredentials = false;
                // smtp.Credentials = new System.Net.NetworkCredential("mais.sangue@hotmail.com", "M@is$angue");

                smtp.Send(message);//Handles all messages in the protocol
            } // Dispose() sends a Quit message 
        }


        public static void Test()
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

            msg.From = new System.Net.Mail.MailAddress("servicedesk@cor-management.ch");
            msg.To.Add(new System.Net.Mail.MailAddress("steiger@cor-management.ch") );

            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Subject = "Hello there";

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.Body = "This is a Test ! äöü ÄÖPÜ & %LOL.";
            
            Test(msg);
        }


        public static bool FOREVER = true;


        public static void StartServer()
        {
            System.Collections.Generic.List<SMTPServer> servers = new System.Collections.Generic.List<SMTPServer>();

            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 25);
            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(endPoint);
            listener.Start();

            while (FOREVER)
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
