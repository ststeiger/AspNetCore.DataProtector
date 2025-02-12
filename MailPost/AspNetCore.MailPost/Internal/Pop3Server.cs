using System;
using System.ComponentModel;
using System.Extensions;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MailPost.Internal
{
	/// <summary>
	///
	/// </summary>
	[LogCategory("AspNetCore.MailPost.Pop3Server")]
	public class Pop3Server : Server
	{
		private readonly IEmailHandle _emailAccount;

		/// <summary>
		///
		/// </summary>
		public override Protocol Protocol { get; } = Protocol.POP3;


		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configuration"></param>
		/// <param name="emailAccount"></param>
		public Pop3Server(ILogger<Pop3Server> logger, IConfiguration configuration, IEmailHandle emailAccount)
			: base(logger, configuration)
		{
			_emailAccount = emailAccount;
		}

		/// <summary>
		///
		/// </summary>
		protected override ServerOptions SetServerOptions()
		{
			ServerOptions serverOptions = ConfigurationExtensions2.TryGet<ServerOptions>(_configuration, Protocol.ToString(), (Action<BinderOptions>)null) ?? new ServerOptions
			{
				Port = 110,
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
		/// <param name="client"></param>
		/// <returns></returns>
		protected override async Task HandleClientAsync(Client client)
		{
			ILogger logger = Globals.GetService<ILoggerFactory>().CreateLogger($"AspNetCore.MailPost.Pop3Server.{client.ConnectionId}");
			Pop3CommandExecutor commandParser = new Pop3CommandExecutor(client, logger, _emailAccount);
			using (FileLoggerExtensions.CreateScope(logger, client.ConnectionId.ToString(), true, true))
			{
				LineBuffer lineBuffer = new LineBuffer();
				await commandParser.SendWelcomeAsync();
				while (!client.CancellationToken.IsCancellationRequested)
				{
					if (!client.TcpClient.Connected)
					{
						FileLoggerExtensions.Warn(logger, $"The {client.TcpClient.Client.RemoteEndPoint} of {client.ConnectionId} is Closed.", "HandleClientAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3Server.cs", (int?)80);
						break;
					}
					byte[] receivedBuffer = new byte[10240];
					int count = await client.TcpClient.Client.ReceiveAsync(receivedBuffer, SocketFlags.None, client.CancellationToken);
					if (count <= 0)
					{
						break;
					}
					for (int i = 0; i < count; i++)
					{
						if (!lineBuffer.Append(receivedBuffer[i]))
						{
							string data = client.Encoding.GetString(lineBuffer.Buffer);
							FileLoggerExtensions.Debug(logger, "reply: \t" + data, "HandleClientAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3Server.cs", (int?)93);
							await commandParser.ExecuteAsync(data);
							lineBuffer.Reset();
						}
					}
				}
				client.Dispose();
			}
		}
	}
}
