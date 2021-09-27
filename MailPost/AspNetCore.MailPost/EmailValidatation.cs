using System;
using System.Net.Mail;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class EmailValidatation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static bool IsValid(string email)
		{
			try
			{
				new MailAddress(email);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}
