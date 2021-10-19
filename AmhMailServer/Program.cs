
namespace AmhMailServer
{


    public class Program
    {

        // https://github.com/cosullivan/SmtpServer
        // https://github.com/hanswolff/simple.mailserver
        // https://github.com/topics/imap-server
        // https://github.com/chibenwa/james-project
        // https://github.com/Nordgedanken/IMAPServer-rs
        // https://github.com/nodemailer/wildduck
        // https://github.com/gmitirol/minimal-imap
        // https://github.com/flashmob/go-guerrilla
        // https://github.com/HaleyLeoZhang/email_server
        // https://github.com/greysoft/mailismus
        // https://github.com/iii-xvi/go-localmail
        // https://github.com/ShahariarRabby/Mail_Server
        // https://github.com/TheFox/imapd
        // https://github.com/QueenieCplusplus/Nginx_Mail
        // https://github.com/mail-in-a-box/mailinabox
        // https://github.com/Malfurious/mailserver
        // https://github.com/htmlgraphic/Mail-Server





        // https://stackoverflow.com/questions/15719207/c-sharp-mail-server
        // https://stackoverflow.com/questions/39555514/how-to-implement-imap-protocol-server-side-using-c-sharp

        // https://en.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol
        // https://en.wikipedia.org/wiki/Internet_Message_Access_Protocol
        // https://en.wikipedia.org/wiki/Push-IMAP
        // Courier only: // https://en.wikipedia.org/wiki/Simple_Mail_Access_Protocol
        // Relay:  a server used for forwarding e-mail
        // https://en.wikipedia.org/wiki/Message_transfer_agent
        // https://en.wikipedia.org/wiki/Network_News_Transfer_Protocol
        // https://en.wikipedia.org/wiki/DNS-based_Authentication_of_Named_Entities
        // https://en.wikipedia.org/wiki/List_of_mail_server_software
        // https://en.wikipedia.org/wiki/Comparison_of_mail_servers
        // https://en.wikipedia.org/wiki/JSON_Meta_Application_Protocol
        // https://en.wikipedia.org/wiki/Category:Internet_mail_protocols


        // SMTP: RFC 2821 
        // https://www.ietf.org/rfc/rfc2821.txt
        // https://datatracker.ietf.org/doc/html/rfc3501
        // https://datatracker.ietf.org/doc/html/rfc5321
        // https://en.wikipedia.org/wiki/Sieve_(mail_filtering_language)
        // https://www.fastmail.help/hc/en-us/articles/1500000280481-Sieve-scripts
        // https://en.wikipedia.org/wiki/Single_point_of_failure



        // https://github.com/seanmcelroy/McNNTP
        // https://github.com/Aldaviva/ImapFolderSubscriptionGuard

        // https://github.com/r0hi7/TrashEmail


        // https://github.com/topics/smpt-server
        // https://github.com/topics/imap-server
        // https://github.com/python-engineer/ml-study-plan


        // (await navigator.storage.estimate()).usage
        // (await navigator.storage.estimate()).quota/(1024*1024*1024)


        public static bool ExecuteWithTimeLimit(System.TimeSpan timeSpan, System.Action codeBlock)
        {
            try
            {
                System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(() => codeBlock());
                if (!task.Wait(timeSpan))
                {
                    // ABORT HERE!
                    System.Console.WriteLine("Time exceeded. Aborted!");
                }

                return task.IsCompleted;
            }
            catch (System.AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }

        } // End Function ExecuteWithTimeLimit 


        private static void UnhandledExceptionTrapper(object sender, System.UnhandledExceptionEventArgs e) 
        { 
            System.Console.WriteLine(e.ExceptionObject.ToString()); 
            System.Console.WriteLine("Press Enter to continue"); 
            System.Console.ReadLine(); 
            System.Environment.Exit(1); 
        } // End Sub UnhandledExceptionTrapper 


        // https://love2dev.com/blog/what-is-the-service-worker-cache-storage-limit/
        // https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API
        // https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps/Offline_Service_workers
        // https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API/Using_Service_Workers
        // https://medium.com/@onejohi/offline-web-apps-using-local-storage-and-service-workers-5d40467117b9
        // https://www.sitepoint.com/offline-web-apps-service-workers-pouchdb/ 
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            // await TestSmtpServer();
            await TestPop3Server();
        }


        public static void SaslTest()
        {
            S22.Sasl.SaslMechanism m = S22.Sasl.SaslFactory.Create("Digest-Md5");
            // Add properties needed by authentication mechanism.
            m.Properties.Add("Username", "Foo");
            m.Properties.Add("Password", "Bar");

            while (!m.IsCompleted)
            {
                // byte[] serverChallenge = GetDataFromServer(...);
                // byte[] clientResponse = m.ComputeResponse(serverChallenge);

                // m.GetResponse()
                
                // SendMyDataToServer(clientResponse);
            }
        }


        public static async System.Threading.Tasks.Task TestPop3Server()
        {
            // SaslTest();
            await StartTestServer();


            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;


            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(TcpPop3Server.Test));
            thread.Start();


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Task Main 


        public static async System.Threading.Tasks.Task TestSmtpServer()
        {
            // await StartTestServer();
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;


            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(TcpSmtpServer.Test));
            thread.Start();

            await System.Threading.Tasks.Task.Delay(100);

            MySmtpClient.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Task Main 


        public static async System.Threading.Tasks.Task StartTestServer()
        {
            MySmtpClient.Test();


            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(MySmtpClient.StartServer));
            thread.Start();


            await System.Threading.Tasks.Task.Delay(100);
            MySmtpClient.Test();


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to stop server --- ");
            System.Console.ReadKey();

            MySmtpClient.StopServer();
        } // End Task StartTestServer 


    } // End Class Program 


} // End Namespace AmhMailServer 
