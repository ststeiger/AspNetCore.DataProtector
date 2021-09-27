using System;
using System.Collections.Generic;
using System.Extensions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public abstract class Server 
		: BackgroundService
	{
		protected class LineBuffer
		{
			private byte? last;

			private List<byte> _bufferContainer = new List<byte>();

			public bool IsEOF { get; private set; }

			public byte[] Buffer { get; private set; } = Array.Empty<byte>();


			public bool Append(byte value)
			{
				if (IsEOF)
				{
					return false;
				}
				if (value == 10 && last == 13)
				{
					IsEOF = true;
					last = value;
					Buffer = new byte[_bufferContainer.Count - 1];
					_bufferContainer.CopyTo(0, Buffer, 0, Buffer.Length);
					return false;
				}
				last = value;
				_bufferContainer.Add(value);
				return true;
			}

			public void Reset()
			{
				_bufferContainer.Clear();
				IsEOF = false;
				last = null;
				Buffer = Array.Empty<byte>();
			}
		}

		/// <summary>
		///
		/// </summary>
		protected const string EOL = "\r\n";

		/// <summary>
		///
		/// </summary>
		protected const int BufferSize = 10240;

		/// <summary>
		///
		/// </summary>
		protected CancellationTokenSource cancellationTokenSource;

		/// <summary>
		///
		/// </summary>
		protected readonly ILogger _logger;

		/// <summary>
		///
		/// </summary>
		protected readonly IConfiguration _configuration;

		/// <summary>
		///
		/// </summary>
		protected ServerOptions _serverOptions;

		/// <summary>
		///
		/// </summary>
		protected readonly List<Client> _clients;

		/// <summary>
		///
		/// </summary>
		public abstract Protocol Protocol { get; }

		/// <summary>
		///
		/// </summary>
		public virtual TcpListener Listener { get; protected set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configuration"></param> 
		public Server(ILogger logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_clients = new List<Client>();
			cancellationTokenSource = new CancellationTokenSource();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			_serverOptions = SetServerOptions();
			try
			{
				Listener = new TcpListener(IPAddress.Parse(_serverOptions.IP), _serverOptions.Port);
				await base.StartAsync(cancellationToken);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				FileLoggerExtensions.Error(_logger, ex, "StartAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)92);
			}
		}

		/// <summary>
		///
		/// </summary>
		protected virtual ServerOptions SetServerOptions()
		{
			ServerOptions serverOptions = ConfigurationExtensions2.TryGet<ServerOptions>(_configuration, Protocol.ToString(), (Action<BinderOptions>)null);
			if (serverOptions == null)
			{
				throw new TypeInitializationException(GetType().FullName, new ArgumentNullException("ServerOptions", "Please config the '" + Protocol.ToString() + "' server first before run it."));
			}
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
				FileLoggerExtensions.Debug(_logger, $"'{GetType().FullName}' starting with '{Protocol}' at '{Listener.LocalEndpoint}'", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)123);
				while (!stoppingToken.IsCancellationRequested)
				{
					TcpClient tcpClient = await Listener.AcceptTcpClientAsync();
					Client client = new Client
					{
						TcpClient = tcpClient,
						ConnectionId = NumberExtensions.NewId((object)this, false),
						CancellationToken = cancellationTokenSource.Token,
						Timeout = _serverOptions.Timeout,
						Protocol = Protocol,
						HostName = _serverOptions.HostName,
						SslCertificate = _serverOptions.SslCertificate,
						Encoding = Encoding.UTF8,
						CreationTime = DateTime.Now,
						OnDispose = RemoveClient
					};
					_clients.Add(client);
					FileLoggerExtensions.Debug(_logger, $"AcceptClient: {client}", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)142);
					ConfigTask(HandleClientAsync(client), client);
				}
				FileLoggerExtensions.Debug(_logger, $"'{GetType().FullName}' cancelled with '{Protocol}' at '{Listener.LocalEndpoint}'", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)145);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				FileLoggerExtensions.Error(_logger, $"An error occurred of '{GetType().FullName}' with {Protocol}.", ex.GetBaseException(), default(EventId), "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)149);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		protected abstract Task HandleClientAsync(Client client);

		private void ConfigTask(Task handle, Client client)
		{
			handle.ContinueWith(delegate(Task c)
			{
				if (c.IsFaulted)
				{
					Exception baseException = c.Exception.GetBaseException();
					FileLoggerExtensions.Error(_logger, JsonExtensions.ToJson((object)new
					{
						Message = baseException.Message,
						RemoteEndPoint = client.TcpClient.Client.RemoteEndPoint.ToString(),
						ConnectionId = client.ConnectionId,
						Protocol = Protocol.ToString(),
						Time = DateTime.Now
					}), baseException, default(EventId), "ConfigTask", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)166);
					client.Dispose();
				}
			});
		}

		private void RemoveClient(Client client)
		{
			int count = _clients.RemoveAll((Client x) => x.ConnectionId == client.ConnectionId);
			if (count > 0)
			{
				FileLoggerExtensions.Debug(_logger, $"{client.ConnectionId} of {client.TcpClient.Client.RemoteEndPoint} disconnected.", "RemoveClient", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)184);
			}
		}

		/// <summary>
		///
		/// </summary>
		public override void Dispose()
		{
			FileLoggerExtensions.Debug(_logger, "'" + GetType().FullName + "' is disposing.", "Dispose", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)192);
			cancellationTokenSource.Cancel();
			_clients.ForEach(delegate(Client x)
			{
				x.Dispose();
			});
			FileLoggerExtensions.Debug(_logger, "'" + GetType().FullName + "' is disposed.", "Dispose", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Server.cs", (int?)198);
		}
	}
}
