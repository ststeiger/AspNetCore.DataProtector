
namespace AmhMailServer
{

    // https://www.codeproject.com/Tips/286952/create-a-simple-smtp-server-in-csharp
    public class SMTPServer
    {

        protected System.Net.Sockets.TcpClient m_client;


        public SMTPServer(System.Net.Sockets.TcpClient client)
        {
            this.m_client = client;
        } // End Constructor 


        private void Write(string strMessage)
        {
            System.Net.Sockets.NetworkStream clientStream = this.m_client.GetStream();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(strMessage + "\r\n");

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        } // End Sub Write 


        private string Read()
        {
            byte[] messageBytes = new byte[8192];
            int bytesRead = 0;
            System.Net.Sockets.NetworkStream clientStream = this.m_client.GetStream();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            bytesRead = clientStream.Read(messageBytes, 0, 8192);
            string strMessage = encoder.GetString(messageBytes, 0, bytesRead);
            return strMessage;
        } // End Function Read 


        public void Run()
        {
            Write("220 localhost -- Fake proxy server");
            string strMessage = string.Empty;

            bool NOSTOP = true;
            /*
            while (NOSTOP)
            {
                try
                {
                    strMessage = Read();
                }
                catch (System.Exception e)
                {
                    //a socket error has occured
                    break;
                }

                if (strMessage.Length > 0)
                {
                    if (strMessage.StartsWith("QUIT"))
                    {
                        this.m_client.Close();
                        break;//exit while
                    }
                    //message has successfully been received
                    if (strMessage.StartsWith("EHLO"))
                    {
                        Write("250 OK");
                    }

                    if (strMessage.StartsWith("RCPT TO"))
                    {
                        Write("250 OK");
                    }

                    if (strMessage.StartsWith("MAIL FROM"))
                    {

                        Write("250 OK");
                    }

                    if (strMessage.StartsWith("DATA"))
                    {
                        Write("354 Start mail input; end with");
                        strMessage = Read();
                        Write("250 OK");
                    }
                }
                else
                {
                    NOSTOP = false; 
                    this.m_client.Close(); 
                } 

            } // Whend 
            */

            System.Console.WriteLine("exiting message loop"); 
        } // End Sub Run 


    } // End Class SMTPServer 


} // End Namespace AmhMailServer 
