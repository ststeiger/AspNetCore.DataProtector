using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class EmailMessage
	{
		/// <summary>
		/// 收件人
		/// </summary>
		public string Owner { get; set; }

		/// <summary>
		/// 邮件发送时间
		/// </summary>
		public DateTime? SendTime { get; set; }

		/// <summary>
		/// Gets or sets the subject line for this email message.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the sender's address for this email message.
		/// </summary>
		public EmailAddress Sender { get; set; }

		/// <summary>
		/// Gets the address collection that contains the recipients of this email message.
		/// </summary>
		public List<EmailAddress> To { get; } = new List<EmailAddress>();


		/// <summary>
		/// Gets or sets the priority of this email message.
		/// </summary>
		public MailPriority Priority { get; set; }

		/// <summary>
		///
		/// </summary>
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the mail message body is in HTML.
		/// </summary>
		public bool IsBodyHtml { get; set; }

		/// <summary>
		/// Gets or sets the encoding used for the user-defined custom headers for this email message.
		/// </summary>
		public string HeadersCharSet { get; set; }

		/// <summary>
		/// Gets the email headers that are transmitted with this email message.
		/// </summary>
		public StringDictionary Headers { get; set; } = new StringDictionary();


		/// <summary>
		/// Gets or sets the from address for this email message.
		/// </summary>
		public EmailAddress From { get; set; }

		/// <summary>
		/// Gets the address collection that contains the carbon copy (CC) recipients for
		/// this email message.
		/// </summary>
		public List<EmailAddress> CC { get; } = new List<EmailAddress>();


		/// <summary>
		///  Gets or sets the encoding used for the subject content for this email message.
		/// </summary>
		public string SubjectCharSet { get; set; }

		/// <summary>
		///  Gets or sets the transfer encoding used to encode the message body.
		/// </summary>
		public TransferEncoding BodyTransferEncoding { get; set; }

		/// <summary>
		/// Gets or sets the encoding used to encode the message body.
		/// </summary>
		public string BodyCharSet { get; set; }

		/// <summary>
		/// Gets or sets the message body.
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		///  Gets the address collection that contains the blind carbon copy (BCC) recipients
		///  for this email message.
		/// </summary>
		public List<EmailAddress> Bcc { get; } = new List<EmailAddress>();


		/// <summary>
		/// Gets the attachment collection used to store data attached to this email message.
		/// </summary>
		public List<EmailAttachment> Attachments { get; } = new List<EmailAttachment>();


		/// <summary>
		/// Gets the attachment collection used to store alternate forms of the message body.
		/// </summary>
		public List<EmailPart> AlternateViews { get; } = new List<EmailPart>();


		/// <summary>
		///
		/// </summary>
		[JsonIgnore]
		public ContentType ContentType { get; set; }

		/// <summary>
		///
		/// </summary>
		public StringBuilder OriginalData { get; set; }
	}
}
