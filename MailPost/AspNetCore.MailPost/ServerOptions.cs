using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public class ServerOptions
	{
		private X509Certificate2 _certificate;

		/// <summary>
		/// 超时时间
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// 监听地址 0.0.0.0
		/// </summary>
		public string IP { get; set; }

		/// <summary>
		/// mail.test.com
		/// </summary>
		public string HostName { get; set; }

		/// <summary>
		/// 监听端口 25
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// ssl证书文件
		/// </summary>
		public string SslCertificatePath { get; set; }

		/// <summary>
		/// ssl证书密码
		/// </summary>
		public string SslCertificatePassword { get; set; }

		/// <summary>
		///
		/// </summary>
		public X509Certificate2 SslCertificate
		{
			get
			{
				if (_certificate == null && !string.IsNullOrEmpty(SslCertificatePath) && File.Exists(SslCertificatePath))
				{
					try
					{
						byte[] bytes = File.ReadAllBytes(SslCertificatePath);
						if (string.IsNullOrEmpty(SslCertificatePassword))
						{
							_certificate = new X509Certificate2(bytes);
						}
						else
						{
							_certificate = new X509Certificate2(bytes, SslCertificatePassword, X509KeyStorageFlags.Exportable);
						}
					}
					catch (Exception)
					{
					}
				}
				return _certificate;
			}
		}
	}
}
