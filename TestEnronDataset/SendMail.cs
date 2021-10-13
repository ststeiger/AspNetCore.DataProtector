
namespace TestEnronDataset
{


    public class SmtpTestClient
    {


        public static async System.Threading.Tasks.Task SendEmailAsync(MimeKit.MimeMessage emailMessage)
        {
            try
            {
                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Timeout = int.MaxValue;

                    // client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                    // client.Authenticate(_settings.Email, _settings.Password);
                    // client.Send(emailMessage);
                    // client.Disconnect(true);

                    // await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.Auto);
                    // await client.ConnectAsync("localhost", 25, MailKit.Security.SecureSocketOptions.Auto);
                    await client.ConnectAsync("127.0.0.1", 25, MailKit.Security.SecureSocketOptions.None);
                    // await client.AuthenticateAsync(_settings.Email, _settings.Password);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception ex) //todo add another try to send email
            {
                if (!"No sender has been specified.".Equals(ex.Message) 
                    && !"No recipients have been specified.".Equals(ex.Message)
                    && !"Collection was modified; enumeration operation may not execute.".Equals(ex.Message)
                    && !"Unrecognized command, 4 retry(ies) remaining.".Equals(ex.Message)
                    )
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
                


                // throw;
            }

        } // End Task SendEmailAsync 


    } // End Class SmtpTestClient 


} // End Namespace TestEnronDataset
