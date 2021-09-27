using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.MailPost
{
	public class MailParser
	{
		private class Content
		{
			public ContentType ContentType { get; set; }

			public string ContentDisposition { get; set; }

			public TransferEncoding TransferEncoding { get; set; } = TransferEncoding.QuotedPrintable;


			public Content Parent { get; set; }

			public string Text { get; set; }

			public string GetContents()
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(Text))
				{
					text = ((TransferEncoding != TransferEncoding.Base64) ? Text : FromBase64(ContentType.CharSet, Text));
				}
				return text;
			}

			public byte[] GetBytes()
			{
				if (!string.IsNullOrEmpty(Text))
				{
					return Convert.FromBase64String(Text);
				}
				return Array.Empty<byte>();
			}
		}

		private static readonly Regex DataEmailRegex = new Regex("\"=\\?([^\\?]+)\\?([^\\?]+)\\?([^\\?]+)\\?=\"\\s*<([^<>]+)>");

		private static readonly Regex ContentRegex = new Regex("=\\?([^\\?]+)\\?([^\\?]+)\\?([^\\?]+)\\?=");

		/// <summary>
		///
		/// </summary>
		/// <param name="email"></param>
		/// <param name="mailData"></param>
		/// <returns></returns>
		public static async ValueTask AnalyzeDataAsync(EmailMessage email, string mailData)
		{
			using (StringReader sr = new StringReader(mailData))
			{
				await ReadHeadersAsync(email, sr);
				string boundary = email.ContentType.Boundary;
				TransferData(email, await ReadDataAsync(sr, boundary));
			}
		}

		private static async Task ReadHeadersAsync(EmailMessage email, StringReader sr)
		{
			string key = string.Empty;
			while (true)
			{
				string data = await sr.ReadLineAsync();
				if (SystemExtensions.IsEmpty(data))
				{
					break;
				}
				if (GetKeyValue(data, ref key, out var value))
				{
					switch (key.ToLower())
					{
					case "from":
					{
						EmailAddress[] mails = GetMailAddresses(value);
						if (mails.Length != 0)
						{
							email.From = mails[0];
						}
						break;
					}
					case "to":
					{
						EmailAddress[] mailAddresses = GetMailAddresses(value);
						foreach (EmailAddress i in mailAddresses)
						{
							email.To.Add(i);
						}
						break;
					}
					case "cc":
					{
						EmailAddress[] mailAddresses2 = GetMailAddresses(value);
						foreach (EmailAddress j in mailAddresses2)
						{
							email.CC.Add(j);
						}
						break;
					}
					case "subject":
					{
						if (SystemExtensions.IsEmpty(value))
						{
							break;
						}
						MatchCollection matches = ContentRegex.Matches(value);
						if (matches.Count == 0)
						{
							email.Subject += value;
							break;
						}
						foreach (Match match in matches)
						{
							email.SubjectCharSet = match.Result("$1");
							email.Subject += FromBase64(email.SubjectCharSet, match.Result("$3"));
						}
						break;
					}
					case "message-id":
						email.MessageId += value?.Trim();
						break;
					case "x-priority":
					case "priority":
					{
						Match k = Regex.Match(value, "(\\d+)");
						if (k.Success)
						{
							email.Priority = (MailPriority)SystemExtensions.ConvertTo<int>((object)k.Result("$1"), "ReadHeadersAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\MailParser.cs", (int?)110);
						}
						break;
					}
					case "date":
						email.SendTime = SystemExtensions.ConvertTo<DateTime>((object)value, "ReadHeadersAsync", "G:\\MyProjects\\AspNetCore.V3.1\\trunk\\Libraries\\AspNetCore.MailPost\\MailParser.cs", (int?)115);
						break;
					case "content-transfer-encoding":
						email.BodyTransferEncoding = GetTransferEncoding(value);
						break;
					default:
						email.Headers[key] += value?.Trim();
						break;
					}
				}
				value = null;
			}
			email.ContentType = new ContentType(email.Headers["content-type"]?.Trim());
		}

		private static async Task<List<Content>> ReadDataAsync(StringReader sr, string boundary, string parentBoundary = null)
		{
			List<Content> contents = new List<Content>();
			while (true)
			{
				string data = await sr.ReadLineAsync();
				if (data == null || data == "." || (!string.IsNullOrEmpty(parentBoundary) && data.EndsWith(parentBoundary + "--")))
				{
					break;
				}
				if (!data.StartsWith("--" + boundary))
				{
					continue;
				}
				while (true)
				{
					IL_019e:
					StringDictionary header = await ReadDataHeaderAsync(sr);
					Content content = new Content
					{
						ContentType = new ContentType(header["content-type"].Trim())
					};
					if (!string.IsNullOrEmpty(content.ContentType.Boundary))
					{
						contents.AddRange(await ReadDataAsync(sr, content.ContentType.Boundary, boundary));
					}
					content.ContentDisposition = header["content-disposition"];
					content.TransferEncoding = GetTransferEncoding(header["content-transfer-encoding"]);
					contents.Add(content);
					while (true)
					{
						string data2 = await sr.ReadLineAsync();
						bool isEnd = data2.EndsWith(boundary + "--");
						if (isEnd && !string.IsNullOrEmpty(parentBoundary))
						{
							return contents;
						}
						if (data2 == null || data2 == "." || isEnd)
						{
							break;
						}
						if (data2.StartsWith("--" + boundary))
						{
							goto IL_019e;
						}
						content.Text += data2;
					}
					break;
				}
			}
			return contents;
		}

		private static async Task<StringDictionary> ReadDataHeaderAsync(StringReader sr)
		{
			StringDictionary header = new StringDictionary();
			string key = string.Empty;
			while (true)
			{
				string data = await sr.ReadLineAsync();
				if (SystemExtensions.IsEmpty(data))
				{
					break;
				}
				if (GetKeyValue(data, ref key, out var value))
				{
					header[key] += value;
				}
				value = null;
			}
			return header;
		}

		private static void TransferData(EmailMessage email, List<Content> contents)
		{
			contents = contents.Where((Content x) => !string.IsNullOrEmpty(x.Text)).ToList();
			Content textContent = contents.FirstOrDefault((Content x) => x.ContentType.MediaType.Equals("text/plain", StringComparison.OrdinalIgnoreCase));
			if (textContent != null)
			{
				email.Body = textContent.GetContents();
				email.AlternateViews.Add(new EmailPart
				{
					Contents = textContent.GetContents(),
					ContentType = textContent.ContentType,
					TransferEncoding = textContent.TransferEncoding
				});
			}

			Content htmlContent = contents.FirstOrDefault((Content x) => x.ContentType.MediaType.Contains("text/html", StringComparison.OrdinalIgnoreCase));
			if (htmlContent != null)
			{
				email.IsBodyHtml = true;
				email.Body = htmlContent.GetContents();
				email.AlternateViews.Add(new EmailPart
				{
					Contents = htmlContent.GetContents(),
					ContentType = htmlContent.ContentType,
					TransferEncoding = htmlContent.TransferEncoding
				});
			}
			foreach (Content c in contents.Where((Content x) => !string.IsNullOrEmpty(x.ContentDisposition)))
			{
				string name = string.Empty;
				foreach (Match match in ContentRegex.Matches(c.ContentType.Name))
				{
					name += FromBase64(c.ContentType.CharSet, match.Result("$3"));
				}
				if (string.IsNullOrEmpty(name))
				{
					name = c.ContentType.Name;
				}
				email.Attachments.Add(new EmailAttachment
				{
					ContentType = c.ContentType,
					TransferEncoding = c.TransferEncoding,
					FileContent = c.GetBytes(),
					FileName = name
				});
			}
		}

		private static EmailAddress[] GetMailAddresses(string value)
		{
			if (SystemExtensions.IsEmpty(value))
			{
				return Array.Empty<EmailAddress>();
			}
			MatchCollection matches = DataEmailRegex.Matches(value);
			List<EmailAddress> mails = new List<EmailAddress>();
			if (matches.Count == 0)
			{
				string[] strs = value.Trim().Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
				if (strs.Length == 0)
				{
					return Array.Empty<EmailAddress>();
				}
				string[] array = strs;
				foreach (string j in array)
				{
					if (EmailAddress.TryParse(j, out var address2))
					{
						mails.Add(address2);
					}
				}
				return mails.ToArray();
			}
			Encoding encoding = default(Encoding);
			foreach (Match i in matches)
			{
				if (Utility.TryGetEncoding(i.Result("$1"), ref encoding) && EmailAddress.TryParse(i.Result("$4"), out var address))
				{
					address.DisplayName = encoding.GetString(Convert.FromBase64String(i.Result("$3")));
					mails.Add(address);
				}
			}
			return mails.ToArray();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool GetKeyValue(string data, ref string key, out string value)
		{
			value = string.Empty;
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			if (data.StartsWith("\t") || data.StartsWith(" "))
			{
				value = data.Trim();
			}
			else
			{
				int pos = data.IndexOf(':');
				if (pos < 0)
				{
					return false;
				}
				key = data.Substring(0, pos);
				value = data.Substring(pos + 1);
			}
			return true;
		}

		private static string FromBase64(string charset, string base64)
		{
			if (string.IsNullOrEmpty(charset))
			{
				return base64;
			}
			if (string.IsNullOrEmpty(base64))
			{
				return string.Empty;
			}
			try
			{
				byte[] bytes = Convert.FromBase64String(base64);
				Encoding encoding = default(Encoding);
				if (Utility.TryGetEncoding(charset, ref encoding))
				{
					return encoding.GetString(bytes);
				}
				return base64;
			}
			catch
			{
				return base64;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static TransferEncoding GetTransferEncoding(string encoding)
		{
			if (string.IsNullOrEmpty(encoding))
			{
				return TransferEncoding.Unknown;
			}
			switch (encoding.Trim().ToLowerInvariant())
			{
			case "8bit":
				return TransferEncoding.EightBit;
			case "7bit":
				return TransferEncoding.SevenBit;
			case "base64":
				return TransferEncoding.Base64;
			case "quotedprintable":
			case "quoted-printable":
				return TransferEncoding.QuotedPrintable;
			default:
				return TransferEncoding.Unknown;
			}
		}
	}
}
