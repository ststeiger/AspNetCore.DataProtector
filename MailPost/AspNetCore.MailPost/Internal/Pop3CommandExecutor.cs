
using System;
using System.Collections.Generic;
using System.Extensions;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace AspNetCore.MailPost.Internal
{
	/// <summary>
	///
	/// </summary>
	internal class Pop3CommandExecutor : CommandExecutor
	{
		/// <summary>
		/// 用户名
		/// </summary>
		public string User { get; private set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		///
		/// </summary>
		public List<InboxEmail> Emails { get; set; } = new List<InboxEmail>();


		/// <summary>
		///
		/// </summary>
		public string HostName { get; set; }

		/// <summary>
		///
		/// </summary>
		public long UserId { get; private set; }

		public Pop3CommandExecutor(Client client, ILogger logger, IEmailHandle emailHandle)
			: base(client, logger, emailHandle)
		{
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override async Task ExecuteAsync(string data)
		{
			string parameter;
			Command command = ParseCommand(data, out parameter);
			switch (command)
			{
			case Command.Unknown:
				FileLoggerExtensions.Fatal(_logger, "Receive [" + data + "]", "ExecuteAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3CommandExecutor.cs", (int?)62);
				await SendCommandAsync("Invalid command in current state", success: false);
				break;
			case Command.Rset:
				Reset();
				Emails.ForEach(delegate(InboxEmail x)
				{
					x.EmailStatus &= (EmailStatus)(-3);
				});
				await SendSuccessAsync();
				break;
			case Command.Noop:
				await SendSuccessAsync();
				break;
			case Command.Capa:
				await SendCommandAsync("CAPA list follows\r\nUSER\r\nUIDL\r\n.");
				break;
			case Command.Help:
				await SendCommandAsync("Normal POP3 commands allowed");
				break;
			case Command.User:
				if (!(await _emailHandle.IsValidEmailAsync(parameter)))
				{
					await SendCommandAsync("Invalid user name.", success: false);
					break;
				}
				HostName = new MailAddress(parameter).Host;
				User = parameter;
				await SendCommandAsync("Send your password.");
				break;
			case Command.Pass:
			{
				long userId = await _emailHandle.IsValidPasswordAsync(User, parameter);
				if (userId <= 0)
				{
					await SendCommandAsync("Invalid password.", success: false);
					break;
				}
				UserId = userId;
				Password = parameter;
				List<InboxEmail> emails = Emails;
				emails.AddRange(await _emailHandle.GetEmailsAsync(userId));
				await SendCommandAsync($"{Emails.Count} message(s), {Emails.Sum((InboxEmail x) => x.EmailSize)} byte(s)");
				break;
			}
			case Command.Stat:
			case Command.List:
			case Command.Retr:
			case Command.Dele:
			case Command.Uidl:
				if (await IsLogon())
				{
					await HandleMessageAsync(command, parameter);
				}
				break;
			case Command.Quit:
				if (UserId > 0)
				{
					long[] messages = (from x in Emails
						where x.EmailStatus.HasFlag(EmailStatus.Deleted)
						select x.Id).ToArray();
					if (messages.Length != 0)
					{
						await _emailHandle.DeleteMessagesAsync(messages);
					}
					await SendSuccessAsync();
					Reset();
				}
				else
				{
					await SendSuccessAsync();
					_client.Dispose();
				}
				break;
			case Command.Stls:
			case Command.Auth:
			case Command.Top:
				break;
			}
		}

		private void Reset()
		{
			UserId = -1L;
			User = string.Empty;
			Password = string.Empty;
			Emails.Clear();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private async ValueTask HandleMessageAsync(Command command, string parameter)
		{
			switch (command)
			{
			case Command.Stat:
				await SendCommandAsync($"{Emails.Count} {Emails.Sum((InboxEmail x) => x.EmailSize)}");
				break;
			case Command.List:
			{
				List<InboxEmail> mails3 = Emails.Where((InboxEmail x) => !x.EmailStatus.HasFlag(EmailStatus.Deleted)).ToList();
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("{0} messages ({1} octets){2}", mails3.Count, mails3.Sum((InboxEmail x) => x.EmailSize), "\r\n");
				int i = 1;
				mails3.ForEach(delegate(InboxEmail m)
				{
					builder.AppendFormat("{0} {1}{2}", i, m.EmailSize, "\r\n");
					i++;
				});
				builder.Append(".");
				await SendCommandAsync(builder.ToString());
				break;
			}
			case Command.Uidl:
			{
				List<InboxEmail> mails4 = Emails.Where((InboxEmail x) => !x.EmailStatus.HasFlag(EmailStatus.Deleted)).ToList();
				int index3 = SystemExtensions.ConvertTo<int>((object)parameter, "HandleMessageAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3CommandExecutor.cs", (int?)180);
				if (index3 > 0)
				{
					try
					{
						mails4 = mails4.Skip(index3 - 1).Take(1).ToList();
						if (mails4.Count == 0)
						{
							await SendCommandAsync("Not found the messages.", success: false);
							return;
						}
					}
					catch
					{
						await SendCommandAsync("Not found the messages.", success: false);
						return;
					}
				}
				StringBuilder builder2 = new StringBuilder();
				builder2.Append("\r\n");
				int j = 1;
				mails4.ForEach(delegate(InboxEmail m)
				{
					builder2.AppendFormat("{0} {1}{2}", j, m.Id, "\r\n");
					j++;
				});
				builder2.Append(".");
				await SendCommandAsync(builder2.ToString());
				break;
			}
			case Command.Retr:
			{
				int index2 = SystemExtensions.ConvertTo<int>((object)parameter, "HandleMessageAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3CommandExecutor.cs", (int?)212);
				if (index2 <= 0)
				{
					await SendCommandAsync("The message not found.", success: false);
					break;
				}
				List<InboxEmail> mails2 = Emails.Where((InboxEmail x) => !x.EmailStatus.HasFlag(EmailStatus.Deleted)).ToList();
				if (index2 > mails2.Count)
				{
					await SendCommandAsync("The message not found.", success: false);
					break;
				}
				byte[] buffer = await _emailHandle.GetEmailMessageAsync(mails2[index2 - 1].Id);
				if (buffer == null || buffer.Length < 1)
				{
					await SendCommandAsync("The message not found.", success: false);
					break;
				}
				await WriteLineAsync($"+OK {buffer.Length}");
				await WriteBytesAsync(buffer);
				await WriteLineAsync("\r\n.");
				break;
			}
			case Command.Dele:
			{
				int index = SystemExtensions.ConvertTo<int>((object)parameter, "HandleMessageAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\Internal\\Pop3CommandExecutor.cs", (int?)237);
				if (index <= 0)
				{
					await SendCommandAsync("The message not found.", success: false);
					break;
				}
				List<InboxEmail> mails = Emails.Where((InboxEmail x) => !x.EmailStatus.HasFlag(EmailStatus.Deleted)).ToList();
				if (index > mails.Count)
				{
					await SendCommandAsync("The message not found.", success: false);
					break;
				}
				mails[index].EmailStatus |= EmailStatus.Deleted;
				await SendSuccessAsync();
				break;
			}
			case Command.Top:
			case Command.Rset:
				break;
			}
		}

		private async ValueTask<bool> IsLogon()
		{
			bool ok = UserId > 0;
			if (!ok)
			{
				await SendCommandAsync("Please login.");
			}
			return ok;
		}

		public override async Task SendSuccessAsync()
		{
			await WriteLineAsync("+OK OK");
		}

		public override async Task SendWelcomeAsync()
		{
			await WriteLineAsync(string.Format(CultureInfo.InvariantCulture, "+OK {0}", _client.Protocol.ToString()));
		}

		private async Task SendCommandAsync(string message, bool success = true)
		{
			await WriteLineAsync((success ? "+OK" : "-ERR") + " " + message);
		}
	}
}
