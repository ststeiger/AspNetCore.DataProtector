using System.Net.Mail;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class EmailAddress
	{
		/// <summary>
		/// Gets the email address specified when this instance was created.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Gets the display name composed from the display name and address information specified when this instance was created.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Gets the host portion of the address specified when this instance was created.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets the user information from the address specified when this instance was created.
		/// </summary>
		public string User { get; set; }

		private string SmtpAddress => "<" + Address + ">";

		public static bool TryParse(string email, out EmailAddress emailAddress)
		{
			emailAddress = null;
			try
			{
				MailAddress i = new MailAddress(email);
				emailAddress = new EmailAddress
				{
					Address = i.Address,
					DisplayName = i.DisplayName,
					Host = i.Host,
					User = i.User
				};
				return true;
			}
			catch
			{
			}
			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(DisplayName))
			{
				return Address;
			}
			return "\"" + DisplayName + "\" " + SmtpAddress;
		}
	}
}
