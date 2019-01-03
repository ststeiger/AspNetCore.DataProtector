/* ===============================================
* 功能描述：AspNetCore.DataProtection.Base256Encoders
* 创 建 者：WeiGe
* 创建日期：9/12/2018 11:50:14 PM
* ===============================================*/

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace AspNetCore.DataProtector
{
    internal  class Base256Encoders
    {
        public const int MaxLegnth = 256;
        static Base256Encoders _Encoder;
        private IOptions<DataProtectorOptions> _options;
        private Base256Encoders(IOptions<DataProtectorOptions> options)
        {
            _options = options;
        }
        internal static void Init(IOptions<DataProtectorOptions> _options)
        {
            _Encoder = new Base256Encoders(_options);
        }
        public static Base256Encoders Instance
        {
            get { return _Encoder; }
        }

        internal  DataProtectionKeys CurrentKey
        {
            get
            {
                if (currentKey == null || currentKey.IsRevoked)
                {
                    var keys = GetKeys();
                    DataProtectionKeys current = keys.FirstOrDefault();
                    if (current == null || current.IsRevoked)
                    {
                        if (current == null && (this._options.Value.AutoGenerateKeys||keys.Count==0))
                        {
                            current = new DataProtectionKeys
                            {
                                CreationDate = DateTime.Now,
                                ExpirationDate = DateTime.Now.Add(this._options.Value.KeyExpired),
                                MasterKey = Base256Encoders.CreateConfusionCodes(),
                                Id = Guid.NewGuid()
                            };
                            File.WriteAllText(
                                Path.Combine(this._options.Value.KeyDirectory.FullName,
                                $"{current.Id.ToString("N")}.json"),
                               JsonConvert.SerializeObject(current));
                            _dataKeys.Insert(0, current);
                        }
                        else
                        {
                            current = new DataProtectionKeys { ExpirationDate = DateTime.Now.AddDays(-1), CreationDate = DateTime.Now.AddDays(-1) };
                        }
                    }
                    currentKey = current;
                }
                return currentKey;
            }
        }
        /// <summary>
        /// 
        /// </summary>
		 List<DataProtectionKeys> _dataKeys = new List<DataProtectionKeys>();
        internal  List<DataProtectionKeys> GetKeys()
        {
            if (_dataKeys.Count == 0)
            {
                var files = this._options.Value.KeyDirectory.GetFiles("*.json");
                List<DataProtectionKeys> keys = new List<DataProtectionKeys>();
                foreach (var f in files)
                {
                    using (var stream = f.OpenText())
                    {
                        string str = stream.ReadToEnd();
                        var key = JsonConvert.DeserializeObject<DataProtectionKeys>(str);
                        if (key.Id==null||key.Id==Guid.Empty)
                        {
                            stream.Close();
                            continue;
                        }
                        if (key.MasterKey.Length == MaxLegnth)
                        {
                            keys.Add(key);
                        }
                    }
                }
                _dataKeys = keys.OrderByDescending(x => x.CreationDate).ToList();
            }
            return _dataKeys;
        }
         DataProtectionKeys currentKey;

        public static readonly UTF8Encoding SecureUtf8Encoding = new UTF8Encoding(false, true);
        //public const string Base256Code0 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789†‡ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſ";
        //public const string Base256Code1 = "!#$%&`()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_'abcdefghijklmnopqrstuvwxyz{|}~ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŢţŤť";
        //public const string Base256Code = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņ";
        //todo unused

        //const string Base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        public static string EncryptToBase64(Guid key, string plainInput, byte[] confusionCode)
        {
            if (string.IsNullOrEmpty(plainInput)||key==null||key==Guid.Empty|| confusionCode.Length!=MaxLegnth)
            {
                return string.Empty;
            }
            byte[] plainBytes = SecureUtf8Encoding.GetBytes(plainInput);
            var encryptBytes = EncryptToBase256(plainBytes, confusionCode);
            var bytes = key.ToByteArray().Concat(encryptBytes);
            return Base64Encoders.Base64Encode(bytes.ToArray());
        }
        public static byte[] EncryptToBase64(Guid key, byte[] plainInput, byte[] confusionCode)
        {
            if (plainInput==null|| plainInput.Length==0 || key == null || key == Guid.Empty || confusionCode.Length != MaxLegnth)
            {
                return Array.Empty<byte>();
            }
            var encryptBytes = EncryptToBase256(plainInput, confusionCode);
            return key.ToByteArray().Concat(encryptBytes).ToArray();
        }
        public static byte[] EncryptToBase256(string plainInput, byte[] confusionCode)
        {
            if (string.IsNullOrEmpty(plainInput))
            {
                return Array.Empty<byte>();
            }

            byte[] plainBytes = SecureUtf8Encoding.GetBytes(plainInput);
            var encryptBytes = EncryptToBase256(plainBytes, confusionCode);
            //var encryptString = new string(output, startIndex: 0, length: output.Length);
            //output = new char[0];
            //return encryptString;
            return encryptBytes;
        }
        public static byte[] EncryptToBase256(byte[] plainBytes, byte[] confusionCode)
        {
            CheckIsBase256Code(confusionCode);
            if (plainBytes == null || plainBytes.Length == 0)
            {
                return new byte[0];
            }           
            byte[] encryptBytes = new byte[plainBytes.Length];
            for (var i = 0; i < plainBytes.Length; i++)
            {
                encryptBytes[i] = confusionCode[plainBytes[i]];
            }
            return encryptBytes;
        }
        public static string DecryptFromBase64(byte[] encryptText, byte[] confusionCode)
        {
            try
            {
                if (encryptText==null|| encryptText.Length==0)
                {
                    return string.Empty;
                }
                var decryptBytes = DecryptFromBase256(encryptText, confusionCode);
                return SecureUtf8Encoding.GetString(decryptBytes);
            }
            catch
            {
                //Logger.Write<Base256Encoders>($"{encryptText}\n{confusionCode}");
                return string.Empty;
            }
        }
        /*
        public static string DecryptFromBase256(string encryptText, byte[] confusionCode)
        {
            if (string.IsNullOrEmpty(encryptText))
            {
                return string.Empty;
            }
            //CheckIsBase256(encryptText);
            byte[] encryptBytes = new byte[encryptText.Length];// SecureUtf8Encoding.GetBytes(encryptText);
            for (var i = 0; i < encryptBytes.Length; i++)
            {
                var index = Base256Code.IndexOf((char)encryptText[i]);
                encryptBytes[i] = (byte)index;
            }
            byte[] decryptBytes = DecryptFromBase256(encryptBytes, confusionCode);
            return SecureUtf8Encoding.GetString(decryptBytes);
        }
        */
        public static byte[] DecryptFromBase256(byte[] encryptBytes, byte[] confusionCode)
        {
            CheckIsBase256Code(confusionCode);
            if (encryptBytes == null || encryptBytes.Length == 0)
            {
                return new byte[0];
            }           
            byte[] decryptBytes = new byte[encryptBytes.Length];
            for (int i = 0; i < encryptBytes.Length; i++)
            {
                //var chars = Base256Code[encryptBytes[i]];
                decryptBytes[i] =(byte)Array.IndexOf( confusionCode, encryptBytes[i]);
                if (decryptBytes[i] < 1)
                {
                    throw new ArgumentOutOfRangeException("Unsupport");
                }
            }
            return decryptBytes;
        }
        /// <summary>
        /// Check whether the the confusion string codes is valid Base256
        /// </summary>
        /// <param name="confusionCode"></param>
        protected static void CheckIsBase256Code(byte[] confusionCode)
        {
            if (confusionCode==null)
            {
                throw new ArgumentNullException(nameof(confusionCode));
            }
            if (confusionCode.Length != MaxLegnth)
            {
                throw new ArgumentException($"Invalid confusion code Length: parameter => confusionCode, length => {confusionCode.Length}");
            }
        }
         
        /// <summary>
        /// Create new confusion string codes
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateConfusionCodes()
        {
            var random = Enumerable.Range(0, MaxLegnth)
                .OrderBy(t => Guid.NewGuid())
                .Select(x=>(byte)x)
                .ToArray();
            return random;
            /*
            var values = string.Join(",", random);
            var codeList = new List<char>(Base256Code.Length);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Base256Code.Length; i++)
            {
                var index = random[i];
                sb.Append(Base256Code[index]);
            }
            return sb.ToString();
            */
        }
    }
}
