namespace AspNetCore.MailPost
{
	/// <summary>
	/// Protocol
	/// </summary>
	public enum Protocol
	{
		/// <summary>
		/// Simple Mail Transfer Protocol
		/// </summary>
		SMTP = 1,
		/// <summary>
		/// Post Office Protocol - Version 3
		/// </summary>
		POP3,
		/// <summary>
		/// Internet Message Access Protocol
		/// </summary>
		IMAP,
		/// <summary>
		/// when Accept mail
		/// </summary>
		Notice
	}
}
