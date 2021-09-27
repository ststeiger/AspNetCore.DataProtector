namespace AspNetCore.MailPost
{
	/// <summary>
	/// 发送状态
	/// </summary>
	public enum SendStatus
	{
		/// <summary>
		/// 发送中
		/// </summary>
		Delivering = 1,
		/// <summary>
		/// 重试中
		/// </summary>
		Retrying = 2,
		/// <summary>
		/// 发送失败
		/// </summary>
		Fatal = 3,
		/// <summary>
		/// 已发送
		/// </summary>
		Delivered = 10
	}
}
