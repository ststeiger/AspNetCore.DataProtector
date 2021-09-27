using System;
using System.Extensions;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCore.MailPost.Internal;
using Microsoft.Extensions.Logging;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	internal class SmtpCommandExecutor : CommandExecutor
	{
		private static readonly Regex HeloEhloRegex = new Regex("^((HELO)|(EHLO)) (.{1,253})$", RegexOptions.IgnoreCase);

		private static readonly Regex EmailRegex = new Regex("<([^<>]+@[^<>]+)>( +([^\\s\\n\\r]+))?");

		private static readonly Regex DataEmailRegex = new Regex("\"=\\?([^\\?]+)\\?([^\\?]+)\\?([^\\?]+)\\?=\"\\s*<([^<>]+)>");

		private static readonly Regex ContentRegex = new Regex("=\\?([^\\?]+)\\?([^\\?]+)\\?([^\\?]+)\\?=");

		private bool readData = false;

		private EmailMessage _currentMail;

		private StringBuilder _data = new StringBuilder();

		/// <summary>
		///
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static string ParseHeloEhlo(string command)
		{
			Match match = HeloEhloRegex.Match(command);
			if (!match.Success)
			{
				return null;
			}
			return match.Result("$4");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string ParseEmail(string command, out string parameters)
		{
			parameters = null;
			Match match = EmailRegex.Match(command);
			if (!match.Success)
			{
				return null;
			}
			parameters = match.Result("$3");
			return match.Result("$1");
		}

		public SmtpCommandExecutor(Client client, ILogger logger, IEmailHandle emailAccount)
			: base(client, logger, emailAccount)
		{
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override async Task ExecuteAsync(string data)
		{
			if (readData)
			{
				await ReadDataAsync(data);
				return;
			}
			string parameter;
			Command command = ParseCommand(data, out parameter);
			if (_currentMail == null)
			{
				_currentMail = new EmailMessage();
			}
			switch (command)
			{
			case Command.Unknown:
				FileLoggerExtensions.Fatal(_logger, "Receive [" + data + "]", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\SmtpCommandExecutor.cs", (int?)105);
				await SendCommandAsync(503, "bad sequence of commands");
				break;
			case Command.Help:
				await SendCommandAsync(211, "211 DATA HELO EHLO MAIL NOOP QUIT RCPT RSET SAML TURN VRFY");
				break;
			case Command.Quit:
				await SendCommandAsync(221, "Bye");
				_client.Dispose();
				break;
			case Command.Noop:
				await SendSuccessAsync();
				break;
			case Command.Helo:
				await SendSuccessAsync();
				break;
			case Command.Ehlo:
			{
				StringBuilder response = new StringBuilder();
				response.AppendFormat("250-{0}{1}", Environment.MachineName, "\r\n");
				response.AppendFormat("250-SIZE{0}", "\r\n");
				response.AppendFormat("250-AUTH LOGIN{0}", "\r\n");
				if (_client.SslCertificate != null)
				{
					response.AppendFormat("250-STARTTLS{0}", "\r\n");
				}
				response.AppendFormat("250 HELP");
				await WriteLineAsync(response.ToString());
				break;
			}
			case Command.StartTls:
				if (_client.SslCertificate != null)
				{
					await SendCommandAsync(220, "Go ahead");
					await new SslStream(_client.TcpClient.GetStream()).AuthenticateAsServerAsync(_client.SslCertificate);
				}
				else
				{
					await SendCommandAsync(503, "bad sequence of commands");
				}
				break;
			case Command.Mail:
			{
				string parameters;
				string email = ParseEmail(parameter, out parameters);
				if (EmailAddress.TryParse(email, out var i))
				{
					_currentMail.From = i;
					_currentMail.Sender = _currentMail.From;
					await SendSuccessAsync();
				}
				else
				{
					await SendCommandAsync(550, "The address is not valid");
				}
				break;
			}
			case Command.Rset:
				readData = false;
				_currentMail = null;
				_data.Clear();
				break;
			case Command.Rcpt:
			{
				string parameters2;
				string email2 = ParseEmail(parameter, out parameters2);
				EmailAddress j;
				bool flag = EmailAddress.TryParse(email2, out j);
				bool flag2 = flag;
				if (flag2)
				{
					flag2 = await ValidEmailAsync(j.Address);
				}
				if (flag2)
				{
					_currentMail.Owner = j.Address;
					_currentMail.To.Add(j);
					await SendSuccessAsync();
				}
				else
				{
					await SendCommandAsync(550, "The address is not valid");
				}
				break;
			}
			case Command.Data:
				readData = true;
				await SendCommandAsync(354, "OK; end with <CRLF>.<CRLF>");
				break;
			}
		}

		private async Task<bool> ValidEmailAsync(string email)
		{
			if (_emailHandle == null)
			{
				FileLoggerExtensions.Error(_logger, "Implementation class of 'IEmailAccount' not found.", "ValidEmailAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\SmtpCommandExecutor.cs", (int?)200);
				await SendCommandAsync(221, "An error occurred from email server.");
				return false;
			}
			if (!(await _emailHandle.IsValidEmailAsync(email)))
			{
				await SendCommandAsync(550, "The address is not valid");
				return false;
			}
			return true;
		}

		private async Task ReadDataAsync(string data)
		{
			if (!string.IsNullOrEmpty(data) && data.Length == 1 && data[0] == '.')
			{
				readData = false;
				await SendSuccessAsync();
				try
				{
					_currentMail.OriginalData = _data;
					await MailParser.AnalyzeDataAsync(_currentMail, _data.ToString());
					FileLoggerExtensions.Info(_logger, JsonExtensions.ToJson((object)_currentMail), "ReadDataAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\SmtpCommandExecutor.cs", (int?)221);
					MailNoticeServer._tcpClients.LastOrDefault((NoticeClient x) => x.Email.Equals(_currentMail.Owner))?.SendEmailAsync(_currentMail.Body);
					await _emailHandle.SaveEmailAsync(_currentMail);
					_currentMail = new EmailMessage();
				}
				catch (Exception ex2)
				{
					Exception ex = ex2;
					FileLoggerExtensions.Error(_logger, _data.ToString(), ex, default(EventId), "ReadDataAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\SmtpCommandExecutor.cs", (int?)234);
				}
			}
			else
			{
				_data.AppendLine(data);
			}
		}

		public override async Task SendWelcomeAsync()
		{
			await WriteLineAsync(string.Format(CultureInfo.InvariantCulture, "220 {0} {1}", _client.HostName, _client.Protocol.ToString()));
		}

		public override async Task SendSuccessAsync()
		{
			await SendCommandAsync(250, "Ok.");
		}

		internal async Task SendCommandAsync(int code, string message)
		{
			await WriteLineAsync($"{code} {message}");
		}
	}
}
