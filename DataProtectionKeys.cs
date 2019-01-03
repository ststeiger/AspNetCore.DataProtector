using System;
using System.Collections.Generic;
namespace AspNetCore.DataProtector
{
    internal class DataProtectionKeys
	{
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
		public DateTime ExpirationDate { get; set; }
		public byte[] MasterKey { get; set; }
        
		[Newtonsoft.Json.JsonIgnore]
		public bool IsRevoked { get { return ExpirationDate < DateTime.Now; } }
	}
}
