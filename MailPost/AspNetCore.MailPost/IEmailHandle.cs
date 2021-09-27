using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	/// <remarks>Please set '<see langword="MailApiHost" />' in config</remarks>
	public interface IEmailHandle
	{
		/// <summary>
		/// 检查用户名,密码是否正确
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		ValueTask<long> IsValidPasswordAsync(string email, string password);

		/// <summary>
		/// 检测邮箱是否正确
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		ValueTask<bool> IsValidEmailAsync(string email);

		/// <summary>
		/// 保存邮件
		/// </summary>
		/// <param name="mail"></param>
		/// <returns></returns>
		ValueTask<bool> SaveEmailAsync(EmailMessage mail);

		/// <summary>
		/// 获取个人邮件
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		ValueTask<List<InboxEmail>> GetEmailsAsync(long accountId);

		/// <summary>
		/// 获取邮件内容
		/// </summary>
		/// <param name="messageId"></param>
		/// <returns></returns>
		ValueTask<byte[]> GetEmailMessageAsync(long messageId);

		/// <summary>
		/// 客户端删信
		/// </summary>
		/// <param name="messageId"></param>
		/// <returns></returns>
		ValueTask<bool> DeleteMessagesAsync(long[] messageId);

		/// <summary>
		/// 获取最新的一份邮件(一般设置为5分钟内)
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		ValueTask<byte[]> GetLastMessageAsync(string email);
	}
}
