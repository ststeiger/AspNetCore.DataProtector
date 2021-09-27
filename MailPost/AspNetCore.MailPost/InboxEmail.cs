using System;
using System.Text;

namespace AspNetCore.MailPost
{
	/// <summary>
	/// 邮件
	/// </summary>
	public class InboxEmail
	{
		/// <summary>
		/// 邮件编号
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// 用户编号
		/// </summary>
		public long AccountId { get; set; }

		/// <summary>
		/// 邮件大小
		/// </summary>
		public long EmailSize { get; set; }

		/// <summary>
		///
		/// </summary>
		public EmailStatus EmailStatus { get; set; }

		/// <summary>
		/// 发送状态
		/// </summary>
		public SendStatus SendStatus { get; set; }

		/// <summary>
		/// 发件人
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// 收件时间
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// 邮件
		/// </summary>
		public StringBuilder Body { get; set; }
	}
}
