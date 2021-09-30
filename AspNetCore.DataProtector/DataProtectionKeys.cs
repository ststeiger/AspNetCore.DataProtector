
namespace AspNetCore.DataProtector
{


    public class DataProtectionKeys
    {
        public System.Guid Id { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public byte[] MasterKey { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public bool IsRevoked
        {
            get
            {
                return ExpirationDate < System.DateTime.Now;
            }
        }


    }


}
