using System;
using System.Extensions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.MailPost.Internal
{
	internal class NoticeClient
	{
		/// <summary>
		///
		/// </summary>
		public long AccountId { get; set; }

		/// <summary>
		///
		/// </summary>
		public TcpClient TcpClient { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///
		/// </summary>
		public string ClientId { get; set; }

		public async Task SendEmailAsync(string content)
		{
			try
			{
				await TcpClient.Client.SendAsync(Encoding.UTF8.GetBytes(content), SocketFlags.None);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				ExceptionExtensions.Log(ex, "NoticeClient.SendEmailAsync", "SendEmailAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\NoticeClient.cs", (int?)48);
			}
		}
	}
}
