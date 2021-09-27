using System;
using AspNetCore.MailPost;
using AspNetCore.MailPost.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	///
	/// </summary>
	public static class MailPostExtensions
	{
		/// <summary>
		/// use mail post
		/// </summary>
		/// <typeparam name="TEmailAccount"></typeparam>
		/// <param name="services"></param>
		/// <param name="mailPostOptions"></param>
		/// <remarks>Please set '<see langword="MailApiHost" />' in config</remarks>
		/// <returns></returns>
		public static IServiceCollection AddMailPost<TEmailAccount>(this IServiceCollection services, Action<MailPostOptions> mailPostOptions = null) where TEmailAccount : class, IEmailHandle
		{
			MailPostOptions mailPost = new MailPostOptions();
			mailPostOptions?.Invoke(mailPost);
			NetClientExtensions.UseNetClient(services);
			services.TryAddSingleton<IEmailHandle, TEmailAccount>();
			if (mailPost.EnableSmtp)
			{
				services.AddHostedService<SmtpServer>();
			}
			if (mailPost.EnablePop3)
			{
				services.AddHostedService<Pop3Server>();
			}
			if (mailPost.EnableNotice)
			{
				services.AddHostedService<MailNoticeServer>();
			}
			return services;
		}
	}
}
