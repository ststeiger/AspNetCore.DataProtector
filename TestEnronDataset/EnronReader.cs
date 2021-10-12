
// Use:
// <PackageReference Include="MimeKitLite" Version="2.11.0" />
// <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
// <PackageReference Include="SharpCompress" Version="0.28.1" /> 

using System.Linq;


namespace TestEnronDataset 
{


    // https://ayende.com/blog/193505-B/working-with-the-enron-dataset
    public class EnronDataReader
    {


        public static void Test()
        {
            // download from https://www.cs.cmu.edu/~./enron/enron_mail_20150507.tar.gz
            string path = @"enron_mail_20150507.tar.gz";
            path = @"D:\username\Downloads\enron_mail_20150507.tar.gz";

            using (System.IO.Stream fs = System.IO.File.OpenRead(path))
            {

                using (SharpCompress.Readers.Tar.TarReader tarReader = SharpCompress.Readers.Tar.TarReader.Open(fs))
                {

                    while (tarReader.MoveToNextEntry())
                    {
                        if (tarReader.Entry.IsDirectory)
                            continue;

                        using (SharpCompress.Common.EntryStream s = tarReader.OpenEntryStream())
                        {
                            MimeKit.MimeMessage msg = MimeKit.MimeMessage.Load(s);
                            Message my = new Message
                            {
                                Bcc = msg.Bcc?.Select(x => x.ToString()).ToList(),
                                Cc = msg.Cc.Select(x => x.ToString()).ToList(),
                                Date = msg.Date,
                                From = msg.From?.Select(x => x.ToString()).ToList(),
                                To = msg.To?.Select(x => x.ToString()).ToList(),
                                References = msg.References?.Select(x => x).ToList(),
                                ReplyTo = msg.ReplyTo?.Select(x => x.ToString()).ToList(),
                                Importance = msg.Importance,
                                InReplyTo = msg.InReplyTo,
                                MessageId = msg.MessageId,
                                Headers = msg.Headers?.GroupBy(x => x.Id).ToDictionary(g => MimeKit.HeaderIdExtensions.ToHeaderName(g.Key), g => g.Select(x => x.Value).ToList()),
                                Priority = msg.Priority,
                                Sender = msg.Sender?.ToString(),
                                Subject = msg.Subject,
                                TextBody = msg.GetTextBody(MimeKit.Text.TextFormat.Plain),
                                XPriority = msg.XPriority
                            };

                            string js = Newtonsoft.Json.JsonConvert.SerializeObject(my, Newtonsoft.Json.Formatting.Indented);
                            System.Console.WriteLine(js);
                        } // End Using s 

                    } // Whend 

                } // End Using tar 

            } // End Using fs 

        } // End Sub Test 


    } // End Class EnronDataReader


} // End Namespace 
