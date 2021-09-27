namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class EmailAttachment : EmailPart
	{
		/// <summary>
		///
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		///
		/// </summary>
		public string FileNameCharSet { get; set; }

		/// <summary>
		///
		/// </summary>
		public byte[] FileContent { get; set; }
	}
}
