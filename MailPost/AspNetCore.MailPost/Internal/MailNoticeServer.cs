using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Extensions;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MailPost.Internal
{
	/// <summary>
	///
	/// </summary>
	[LogCategory("AspNetCore.MailPost.MailNoticeServer")]
	public class MailNoticeServer : Server
	{
		internal static ConcurrentBag<NoticeClient> _tcpClients = new ConcurrentBag<NoticeClient>();

		private readonly IEmailHandle _emailHandle;

		/// <summary>
		///
		/// </summary>
		public override Protocol Protocol => Protocol.Notice;

		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configuration"></param>
		/// <param name="emailHandle"></param>
		public MailNoticeServer(ILogger<MailNoticeServer> logger, IConfiguration configuration, IEmailHandle emailHandle)
			: base(logger, configuration)
		{
			_emailHandle = emailHandle;
		}

		/// <summary>
		///
		/// </summary>
		protected override ServerOptions SetServerOptions()
		{
			ServerOptions serverOptions = ConfigurationExtensions2.TryGet<ServerOptions>(_configuration, Protocol.ToString(), (Action<BinderOptions>)null) ?? new ServerOptions
			{
				Port = 4000,
				IP = "0.0.0.0"
			};
			if (serverOptions.Timeout.TotalSeconds <= 1.0)
			{
				serverOptions.Timeout = TimeSpan.FromSeconds(60.0);
			}
			return serverOptions;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				Listener.Start();
				FileLoggerExtensions.Debug(_logger, $"'{GetType().FullName}' starting with '{Protocol}' at '{Listener.LocalEndpoint}'", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\MailNoticeServer.cs", (int?)76);
				while (!stoppingToken.IsCancellationRequested)
				{
					HandleClientAsync(await Listener.AcceptTcpClientAsync());
				}
				FileLoggerExtensions.Debug(_logger, $"'{GetType().FullName}' cancelled with '{Protocol}' at '{Listener.LocalEndpoint}'", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\MailNoticeServer.cs", (int?)83);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				FileLoggerExtensions.Error(_logger, $"An error occurred of '{GetType().FullName}' with {Protocol}.", ex.GetBaseException(), default(EventId), "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\MailNoticeServer.cs", (int?)87);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		protected async Task HandleClientAsync(TcpClient client)
		{
			try
			{
				while (true)
				{
					byte[] receivedBuffer = new byte[10240];
					int count = await client.Client.ReceiveAsync(receivedBuffer, SocketFlags.None, cancellationTokenSource.Token);
					if (count > 0)
					{
						string result = Encoding.UTF8.GetString(receivedBuffer, 0, count);
						Match match = Regex.Match(result, "^([^\\r\\n\\s]+)\\t([^\\r\\n\\s]+)(\\t([^\\r\\n\\s]+))?\r\n$", RegexOptions.IgnoreCase);
						if (!match.Success)
						{
							await client.Client.SendAsync(Encoding.UTF8.GetBytes("Invalid arguments."), SocketFlags.None);
							client.Close();
							return;
						}
						NoticeClient noticeClient = new NoticeClient
						{
							TcpClient = client,
							Email = match.Result("$1"),
							Password = match.Result("$2"),
							ClientId = match.Result("$3")
						};
						long userId = await _emailHandle.IsValidPasswordAsync(noticeClient.Email, noticeClient.Password);
						if (userId <= 0)
						{
							break;
						}
						noticeClient.AccountId = userId;
						_tcpClients.Add(noticeClient);
					}
				}
				await client.Client.SendAsync(Encoding.UTF8.GetBytes("Invalid email or password."), SocketFlags.None);
				client.Close();
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				FileLoggerExtensions.Error(_logger, ex, "HandleClientAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\MailNoticeServer.cs", (int?)134);
			}
		}

		protected override Task HandleClientAsync(Client client)
		{
			throw new NotImplementedException();
		}
	}
}
