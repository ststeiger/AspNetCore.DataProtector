namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public enum EmailStatus
	{
		/// <summary>
		/// 已查看
		/// </summary>
		Seen = 1,
		/// <summary>
		/// 已删除
		/// </summary>
		Deleted = 2,
		/// <summary>
		/// 已标记
		/// </summary>
		Flagged = 4,
		/// <summary>
		/// 已回复
		/// </summary>
		Answered = 8,
		/// <summary>
		/// 草稿
		/// </summary>
		Draft = 0x10,
		/// <summary>
		/// 最新
		/// </summary>
		Recent = 0x20,
		/// <summary>
		/// 病毒扫描
		/// </summary>
		VirusScan = 0x40
	}
}
