
namespace TestEnronDataset
{


    // https://ayende.com/blog/193505-B/working-with-the-enron-dataset 
    public class Message
    {
        public MimeKit.MessagePriority Priority { get; set; }

        public MimeKit.XMessagePriority XPriority { get; set; }

        public string Sender { get; set; }


        public System.Collections.Generic.List<string> From { get; set; }

        public System.Collections.Generic.List<string> ReplyTo { get; set; }

        public System.Collections.Generic.List<string> To { get; set; }
        public System.Collections.Generic.List<string> Cc { get; set; }

        public MimeKit.MessageImportance Importance { get; set; }

        public string Subject { get; set; }
        public System.DateTimeOffset Date { get; set; }

        public System.Collections.Generic.List<string> References { get; set; }
        public string InReplyTo { get; set; }
        public string MessageId { get; set; }

        public string TextBody { get; set; }

        public System.Collections.Generic.List<string> Bcc { get; set; }
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> Headers { get; set; }

    }


}
