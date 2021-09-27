using System.Net.Mime;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class EmailPart
	{
		/// <summary>
		///
		/// </summary>
		public ContentType ContentType { get; set; }

		/// <summary>
		///
		/// </summary>
		public string Contents { get; set; }

		/// <summary>
		///
		/// </summary>
		public TransferEncoding TransferEncoding { get; set; }
	}
}
