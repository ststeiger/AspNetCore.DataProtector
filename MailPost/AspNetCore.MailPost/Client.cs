using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public struct Client : IDisposable
	{
		private bool _isDisposed;

		/// <summary>
		///
		/// </summary>
		public Encoding Encoding
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public string HostName
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public Protocol Protocol
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public TimeSpan Timeout
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public TcpClient TcpClient
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public DateTime CreationTime
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public long ConnectionId
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public CancellationToken CancellationToken
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		public X509Certificate2 SslCertificate
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		internal Action<Client> OnDispose
		{
			// [IsReadOnly]
			get;
			set; }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{TcpClient.Client.RemoteEndPoint} with {ConnectionId}";
		}

		/// <summary>
		///
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}
			try
			{
				if (TcpClient.Connected)
				{
					TcpClient.Client.Dispose();
					TcpClient.Close();
				}
			}
			catch
			{
			}
			OnDispose?.Invoke(this);
			_isDisposed = true;
		}
	}
}
