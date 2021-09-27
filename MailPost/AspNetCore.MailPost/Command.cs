namespace AspNetCore.MailPost
{
	/// <summary>
	///
	/// </summary>
	public enum Command
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// No operation
		/// </summary>
		Noop,
		/// <summary>
		///
		/// </summary>
		Stls,
		/// <summary>
		/// user name
		/// </summary>
		User,
		/// <summary>
		/// password
		/// </summary>
		Pass,
		/// <summary>
		/// Authentication
		/// </summary>
		Auth,
		/// <summary>
		/// help
		/// </summary>
		Help,
		/// <summary>
		/// quit
		/// </summary>
		Quit,
		/// <summary>
		/// Number and total size of all messages
		/// </summary>
		Stat,
		/// <summary>
		/// list email
		/// </summary>
		List,
		/// <summary>
		/// Retrieve selected message
		/// </summary>
		Retr,
		/// <summary>
		///
		/// </summary>
		Top,
		/// <summary>
		/// Reset the mailbox. Undelete deleted messages.
		/// </summary>
		Rset,
		/// <summary>
		/// Delete selected message
		/// </summary>
		Dele,
		/// <summary>
		///
		/// </summary>
		Uidl,
		/// <summary>
		///
		/// </summary>
		Capa,
		/// <summary>
		/// Hello
		/// </summary>
		Helo,
		/// <summary>
		/// Extended Hello
		/// </summary>
		Ehlo,
		/// <summary>
		/// Start Transport Layer Security
		/// </summary>
		StartTls,
		/// <summary>
		///
		/// </summary>
		Mail,
		/// <summary>
		/// Send email to recipient
		/// </summary>
		Rcpt,
		/// <summary>
		///
		/// </summary>
		Data,
		/// <summary>
		/// Verify
		/// </summary>
		Vrfy,
		/// <summary>
		///
		/// </summary>
		Size,
		/// <summary>
		/// Send and mail
		/// </summary>
		Saml,
		/// <summary>
		/// Send
		/// </summary>
		Send,
		/// <summary>
		/// Send or mail
		/// </summary>
		Soml,
		/// <summary>
		/// Turn
		/// </summary>
		Turn,
		/// <summary>
		/// 以创建指定名字的新邮箱。邮箱名称通常是带路径的文件夹全名
		/// </summary>
		Create,
		/// <summary>
		/// 删除指定名字的文件夹。文件夹名字通常是带路径的文件夹全名，当邮箱被删除后，其中的邮件也不复存在。
		/// </summary>
		Delete,
		/// <summary>
		/// 修改文件夹的名称，它使用两个参数：当前邮箱名和新邮箱名，两个参数的命名符合标准路径命名规则。
		/// </summary>
		Rename,
		/// <summary>
		/// 上载一个邮件到指定的Folder（文件夹/邮箱）中。命令中包含了新邮件的属性、日期/时间、大小，随后是邮件数据        /// 
		/// </summary>
		/// <remarks>
		/// folder,attributes,date/time,size,mail data
		/// </remarks>
		Append,
		/// <summary>
		/// 选定某个邮箱（Folder），表示即将对该邮箱（Folder）内的邮件作操作。邮箱标志的当前状态也返回给了用户，同时返回的还有一些关于邮件和邮箱的附加信息。
		/// </summary>
		Select,
		/// <summary>
		/// 命令用于读取邮件的文本信息，且仅用于显示的目的。包含两个参数，messageset：表示希望读取的邮件号列表，
		/// IAMP服务器邮箱中的每个邮件都有 一个唯一的ID标识，（邮件号列表参数可以是一个邮件号，也可以是由逗号分隔的多个邮件号，
		/// 或者由冒号间隔的一个范围），IMAP服务器返回邮件号列表中 全部邮件的指定数据项内容。
		/// </summary>
		/// <remarks>
		/// ALL：只返回按照一定格式的邮件摘要，包括邮件标志、RFC822.SIZE、自身的时间和信封信息。IMAP客户机能够将标准邮件解析成这些信息并显示出来。
		///             BODY：只返回邮件体文本格式和大小的摘要信息。IMAP客户机可以识别这些细腻，并向用户显示详细的关于邮件的信息。其实是一些非扩展的BODYSTRUCTURE的信息。
		///             FAST：只返回邮件的一些摘要，包括邮件标志、RFC822.SIZE、和自身的时间。
		///             FULL：同样的还是一些摘要信息，包括邮件标志、RFC822.SIZE、自身的时间和BODYSTRUCTURE的信息。
		///             BODYSTRUCTUR： 是邮件的[MIME-IMB]的体结构。这是服务器通过解析[RFC-2822]头中的[MIME-IMB]各字段和[MIME-IMB]头信息得出来 的。包括的内容有：邮件正文的类型、字符集、编码方式等和各附件的类型、字符集、编码方式、文件名称等等。
		///             ENVELOPE：信息的信封结构。是服务器通过解析[RFC-2822]头中的[MIME-IMB]各字段得出来的，默认各字段都是需要的。主要包括：自身的时间、附件数、收件人、发件人等。
		///             FLAGS：此邮件的标志。
		///             INTERNALDATE：自身的时间。
		///             RFC822.SIZE：邮件的[RFC-2822]大小
		///             RFC822.HEADER：在功能上等同于BODY.PEEK[HEADER]，
		///             RFC822：功能上等同于BODY[]。
		///             RFC822.TEXT：功能上等同于BODY[TEXT]
		///             UID：返回邮件的UID号，UID号是唯一标识邮件的一个号码。
		///             BODY[section] partial：返回邮件的中的某一指定部分，返回的部分用section来表示，section部分包含的信息通常是 代表某一部分的一个数字或者是下面的某一个部分：HEADER, HEADER.FIELDS, HEADER.FIELDS.NOT, MIME, and TEXT。如果section部分是空的话，那就代表返回全部的信息，包括头信息。
		///             BODY[HEADER]返回完整的文件头信息。
		///             BODY[HEADER.FIELDS ()]：在小括号里面可以指定返回的特定字段。
		///             BODY[HEADER.FIELDS.NOT ()]：在小括号里面可以指定不需要返回的特定字段。
		///             BODY[MIME]：返回邮件的[MIME-IMB]的头信息，在正常情况下跟BODY[HEADER]没有区别。
		///             BODY[TEXT]：返回整个邮件体，这里的邮件体并不包括邮件头。
		///             现在我们遇到了一个问题，如果我们要单独提取邮件的附件怎么办？
		///             通过以上的命令我们是无法做到的，但是我们别忘了在section部分还有其他的方式可以来表示我们要提取的邮件的部分，那就的通过区段数来表示。那下面就让我们来看看什么是区段数。
		///             每 个邮件都至少有一个区段数，Non-[MIME-IMB]型的邮件和non-multipart [MIME-IMB]的邮件是没有经过MIME编码之后的信息的，那这样的信息只有一个区段数1。多区段型的信息被编排成一个连续的区段数，这和实际信息 里出现的是一样的。如果一个特定的区段有类型信息或者是多区段的，一个MESSAGE/RFC822类型的区段也含有嵌套的区段数，这些区段数是指向这些 信息区段的信息体的。
		///             说了那么多拗口的，现在我们讲的更简单易懂一些。在一个邮件体里面，区段数1代表的邮件的正文，区段数二代表的是第一个附 件，区段数三代表的是第二个附件，以此类推。在这些区段里，如果有哪个区段又是多区段的，比如2区段的内容格式是mulipart或者是 MESSAGE/RFC822类型的，那么这个区段又嵌套了多个子区段，嵌套的各子区段是用2.1，2.2……等等表示，类似，如果2.1又有嵌套，那么 还会有2.1.1，2.1.2等区段。这样的嵌套是没有限制的。下面我们通过例子来了解一下fetch具体是怎么按区段下载的。
		///             HEADER ([RFC-2822] header of the message)
		///             TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
		///             1 TEXT/PLAIN
		///             2 APPLICATION/OCTET-STREAM
		///             3 MESSAGE/RFC822
		///             3.HEADER ([RFC-2822] header of the message)
		///             3. TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
		///             3.1 TEXT/PLAIN
		///             3.2 APPLICATION/OCTET-STREAM
		///             4 MULTIPART/MIXED
		///             4.1 IMAGE/GIF
		///             4.1. MIME ([MIME-IMB] header for the IMAGE/GIF)
		///             4.2 MESSAGE/RFC822
		///             4.2. HEADER ([RFC-2822] header of the message)
		///             4.2. TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
		///             4.2.1 TEXT/PLAIN
		///             4.2.2 MULTIPART/ALTERNATIVE
		///             4.2.2.1 TEXT/PLAIN
		///             4.2.2.2 TEXT/RICHTEXT
		///             如果我们需要取第一个附件，那么命令就是：
		///             C:a2 fetch 4 body[2];
		///             取第三个区段的第一个子区段文本正文，命令就是：
		///             C:a2 fetch 4 body[3.1];
		///             取第四个区段的第二个子区段再嵌套的第一个子区段的文本正文，命令如下：
		///             C:a2 fetch 4 body[4.2.1]
		///             当然这个例子只是针对于一个特殊的邮件结构，一般的邮件应该都没有这么复杂的结构。
		///             再 接下来我们再看看最后一个参数有什么用？BODY[section]可以使用partial字段进行修改，该字段包含两个用“.”隔开的数字，第一个数 字、是八进制表示的希望显示的数据输出起始位置，第二个数字是八进制表示希望显示的数据长度。这项功能可以进一步设定输出格式，例如，如果你希望显示1号 邮件中邮件提的前1500个字符，可以使用命令：
		///             FETCH 1 BODY[TEXT] 0.1500
		///             该命令取回邮件提的前1500个字符并定义为TEXT，如果邮件体少于1500个字符则返回整个邮件体。
		///             例：
		///             C: 100 FETCH 3：5  BODY[header.fields (Date From Subject)]  /*冒号表示间隔的一个范围：请求邮件从3到5之间所有邮件的Date：字段、 From：字段和 Subject：字段的信息*/
		///             S: *  3  FETCH  (BODY[HEADER.FIELDS (“DATE” “FROM” “SUBJECT”)]  {112}
		///             DATE: Tue, 14 Sep 1999 10:09:50 -500
		///             From: alex@shadrach.smallorg.org
		///             Subject: This is the first test message
		///             )
		///             S: *  4  FETCH  (BODY[HEADER.FIELDS (“DATE” “FROM” “SUBJECT”)]  {113}
		///             DATE: Tue, 14 Sep 1999 10:10:04 -500
		///             From: alex@shadrach.smallorg.org
		///             Subject: This is the second test message
		///             )
		///             S: *  5  FETCH  (BODY[HEADER.FIELDS (“DATE” “FROM” “SUBJECT”)]  {112}
		///             DATE: Tue, 14 Sep 1999 10:20:26 -500
		///             From: alex@shadrach.smallorg.org
		///             Subject: This is the first test message
		///             S: A100 OK FETCH completed
		///             C: A101 FETCH BODY[TEXT]
		///             S:* This is the fourth test message for IMAP
		///             S: A101 OK FETCH completed
		///             FETCH命令是IMAP协议里最复杂的命令。FETCH的命令参数很多、很复杂，但基本的特征是允许将邮件按照MIME结构拆解为零碎的部件来提取。例如，可以利用FETCH命令提取邮件头、某一个附件、或某一邮件附件头部的某一字段，等等。
		///             BODY.PEEK [section] partial：
		///             在 缺省设置时，宏BODY[section] [partial]会设置邮件的\SEEN标志。如果你想在不设 置\SEEN标志的情况下阅读邮件的部分信息，那么可以将该宏替代BODY .PEEK[section]，后者完成同前者一样的功能但不会设置该邮件的\SEEN标志。
		///             </remarks>
		Fetch,
		/// <summary>
		/// 修改指定邮件的属性，包括给邮件打上已读标记、删除标记，等等
		/// [mail id][new attributes]
		/// </summary>
		Store
	}
}
