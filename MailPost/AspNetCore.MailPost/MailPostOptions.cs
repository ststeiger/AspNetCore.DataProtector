namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class MailPostOptions
	{
		/// <summary>
		/// <para><see langword="default is enabled." /></para>
		/// Add smtp server use config <see cref="T:AspNetCore.MailPost.ServerOptions" /> of named <see langword="Smtp" />.
		/// <para>default port is <see langword="25" /></para>
		/// </summary>
		public bool EnableSmtp { get; set; } = true;


		/// <summary>
		/// Add smtp server use config <see cref="T:AspNetCore.MailPost.ServerOptions" /> of named <see langword="Pop3" />.
		/// <para>default port is <see langword="110" /></para>
		/// </summary>
		public bool EnablePop3 { get; set; }

		/// <summary>
		/// Add smtp server use config <see cref="T:AspNetCore.MailPost.ServerOptions" /> of named <see langword="Notice" />.
		/// <para>default port is <see langword="4000" /></para>
		/// </summary>
		public bool EnableNotice { get; set; }
	}
}
