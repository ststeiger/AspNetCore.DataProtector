using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public abstract class CommandExecutor
	{
		/// <summary>
		///
		/// </summary>
		protected const string EOF = "\r\n";

		/// <summary>
		///
		/// </summary>
		protected static readonly Dictionary<string, Command> _commands;

		/// <summary>
		///
		/// </summary>
		protected readonly Client _client;

		/// <summary>
		///
		/// </summary>
		protected readonly ILogger _logger;

		/// <summary>
		///
		/// </summary>
		protected readonly IEmailHandle _emailHandle;

		static CommandExecutor()
		{
			_commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
			Array values = Enum.GetValues(typeof(Command));
			foreach (object value in values)
			{
				_commands[value.ToString()] = (Command)value;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="client"></param>
		/// <param name="logger"></param>
		/// <param name="emailHandle"></param>
		public CommandExecutor(Client client, ILogger logger, IEmailHandle emailHandle)
		{
			_client = client;
			_logger = logger;
			_emailHandle = emailHandle;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public abstract Task SendWelcomeAsync();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public abstract Task SendSuccessAsync();

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public abstract Task ExecuteAsync(string data);

		/// <summary>
		///
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		protected virtual async Task ExecuteWithTimeout(Func<Task> task)
		{
			CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();
			Task timeoutTask = Task.Delay(_client.Timeout, timeoutCancellationTokenSource.Token);
			await Task.WhenAny(timeoutTask, task());
			if (timeoutTask.IsCompleted)
			{
				throw new TimeoutException($"The timeout has expired. ({_client.Timeout})");
			}
			timeoutCancellationTokenSource.Cancel();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		protected virtual async Task WriteLineAsync(string message)
		{
			FileLoggerExtensions.Debug(_logger, "send:\t" + message, "WriteLineAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\CommandExecutor.cs", (int?)106);
			byte[] bytes = _client.Encoding.GetBytes(message + "\r\n");
			TcpClient tcp = _client.TcpClient;
			CancellationToken cancellationToken = _client.CancellationToken;
			await ExecuteWithTimeout(async delegate
			{
				await tcp.Client.SendAsync(bytes, SocketFlags.None, cancellationToken);
			});
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		protected virtual async Task WriteBytesAsync(byte[] buffer)
		{
			FileLoggerExtensions.Debug(_logger, $"send:\tbuffer size:{buffer.Length}", "WriteBytesAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\CommandExecutor.cs", (int?)122);
			TcpClient tcp = _client.TcpClient;
			CancellationToken cancellationToken = _client.CancellationToken;
			await ExecuteWithTimeout(async delegate
			{
				int length = 4096;
				int total = buffer.Length;
				for (int start = 0; start < total; start += length)
				{
					byte[] reducedBuffer = ((total - start <= length) ? new byte[total - start] : new byte[length]);
					Buffer.BlockCopy(buffer, start, reducedBuffer, 0, reducedBuffer.Length);
					await tcp.Client.SendAsync(reducedBuffer, SocketFlags.None, cancellationToken);
				}
			});
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public virtual Command ParseCommand(string command, out string parameter)
		{
			parameter = string.Empty;
			if (string.IsNullOrWhiteSpace(command))
			{
				return Command.Unknown;
			}
			int commandPosition = command.IndexOf(' ');
			if (commandPosition == -1)
			{
				if (_commands.TryGetValue(command, out var cmd))
				{
					return cmd;
				}
				return Command.Unknown;
			}
			string key = command.Substring(0, commandPosition);
			if (_commands.TryGetValue(key, out var value))
			{
				parameter = command.Substring(commandPosition + 1).Trim();
				return value;
			}
			return Command.Unknown;
		}
	}
}
