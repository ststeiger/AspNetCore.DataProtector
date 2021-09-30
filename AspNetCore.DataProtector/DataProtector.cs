/* ===============================================
* Function description: AspNetCore.DataProtection.DataProtector
* Creator: WeiGe
* Creation date: 9/12/2018 11:51:52 PM
* ===============================================*/

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.IO;
using System.Linq;
using System.IO.Compression;


namespace AspNetCore.DataProtector
{


    internal class DataProtector
        : IDataProtector
    {
        protected const int KeyIdLength = 16;
        protected static readonly byte[] EmptyBytes = new byte[0];
        protected ILogger<DataProtector> _logger;
        protected DataProtectorOptions _options;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        internal DataProtector(IOptions<DataProtectorOptions> options, ILogger<DataProtector> logger)
        {
            _logger = logger;

            Base256Encoders.Init(options);
            if (Base256Encoders.Instance.CurrentKey == null)
            {
                if (_logger != null)
                    _logger.LogDebug("Cannot generate the Key");

                throw new Exception("Cannot generate the Key");
            }
            _options = options.Value;
        }


        internal DataProtector(DataProtectorOptions options)
            :this(Microsoft.Extensions.Options.Options.Create(options), null)
        { }
        

        public byte[] FromBase64(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
            {
                return EmptyBytes;
            }

            return Base64Encoders.Base64Decode(base64Text);
        }


        public string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }
            
            byte[] bytes = Base256Encoders.SecureUtf8Encoding.GetBytes(plainText);
            bytes = this.Protect(bytes);

            if (bytes.Length == 0)
            {
                if (_logger != null)
                    _logger.LogWarning($"Key not found for Protect: '{plainText}'.");
            }

            return this.ToBase64(bytes);
        }


        public byte[] Protect(byte[] plainData)
        {
            if (plainData == null || plainData.Length == 0)
            {
                return EmptyBytes;
            }
            
            DataProtectionKeys key = Base256Encoders.Instance.CurrentKey;
            
            if (key == null || key.Id == null || key.Id == Guid.Empty
                || key.MasterKey == null
                || key.MasterKey.Length != Base256Encoders.MaxLegnth)
            {
                if (_logger != null)
                    _logger.LogWarning($"Key not found for Protect: [{string.Join(",", plainData)}].");

                return EmptyBytes;
            }
            
            plainData = ToRandom(plainData, out byte random);
            byte[] encryptBytes = Base256Encoders.EncryptToBase256(plainData, key.MasterKey);

            if (this._options.UseCompress)
            {
                using (System.IO.MemoryStream memoryStream = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                    {
                        deflateStream.Write(encryptBytes, 0, encryptBytes.Length);
                    }

                    byte[] bytes = key.Id.ToByteArray().Concat(new byte[] { random }).Concat(memoryStream.ToArray()).ToArray();
                    if (_logger != null)
                        _logger.LogDebug($"Use Key '{key.Id}' Protect with compress.");

                    return bytes;
                }
            }
            else
            {
                byte[] bytes = key.Id.ToByteArray().Concat(new byte[] { random }).Concat(encryptBytes).ToArray();

                if(_logger != null)
                    _logger.LogDebug($"Use Key '{key.Id}' Protect  without compress.");

                return bytes;
            }
        }


        public string ToBase64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }

            return Base64Encoders.Base64Encode(bytes);
        }


        public string Unprotect(string protectedData)
        {
            if (string.IsNullOrEmpty(protectedData))
            {
                return string.Empty;
            }

            byte[] bytes = this.FromBase64(protectedData);
            byte[] datas = this.Unprotect(bytes);

            return Base256Encoders.SecureUtf8Encoding.GetString(datas);
        }


        public byte[] Unprotect(byte[] protectedData)
        {
            if (protectedData == null || protectedData.Length < KeyIdLength + 1)
            {
                return EmptyBytes;
            }

            Guid id;
            try
            {
                id = new Guid(protectedData.Take(KeyIdLength).ToArray());
            }
            catch
            {
                return EmptyBytes;
            }

            DataProtectionKeys key = Base256Encoders.Instance.GetKeys().FirstOrDefault(t => t.Id == id);
            if (key == null)
            {
                if(_logger != null)
                    _logger.LogWarning($"Key not found to Unprotect.");

                return EmptyBytes;
            }

            byte random = protectedData.Skip(KeyIdLength).FirstOrDefault();
            byte[] data = protectedData.Skip(KeyIdLength + 1).ToArray();

            if (data.Length <= 0)
            {
                return EmptyBytes;
            }

            byte[] bytes;
            if (this._options.UseCompress)
            {
                if (_logger != null)
                    _logger.LogDebug($"Use Key '{key.Id}' Unprotect with compress.");

                using (System.IO.MemoryStream memoryStream = new MemoryStream(data))
                {
                    using (System.IO.MemoryStream memoryStream1 = new MemoryStream())
                    {
                        using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                        {
                            deflateStream.CopyTo(memoryStream1);
                        }

                        bytes = memoryStream1.ToArray();
                    }
                }
            }
            else
            {
                bytes = data;
                if (_logger != null)
                    _logger.LogDebug($"Use Key '{key.Id}' Unprotect without compress.");
            }

            byte[] _data = Base256Encoders.DecryptFromBase256(bytes, key.MasterKey);
            FromRandom(random, _data);
            return _data;
        }


        protected byte GetRandom()
        {
            byte random = (byte)Enumerable.Range(1, KeyIdLength * 2).OrderBy(t => Guid.NewGuid()).FirstOrDefault();
            return random;
        }


        byte[] ToRandom(byte[] bytes, out byte random)
        {
            random = GetRandom();
            for (int index = 0; index < bytes.Length; index++)
            {
                int x = bytes[index] + random;
                if (x > byte.MaxValue)
                {
                    x = x - byte.MaxValue;
                }

                bytes[index] = (byte)x;
            }

            return bytes;
        }


        byte[] FromRandom(byte random, byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; index++)
            {
                int x = bytes[index] - random;
                if (x < 0)
                {
                    x = byte.MaxValue + x;
                }

                bytes[index] = (byte)x;
            }

            return bytes;
        }


    } // End Class DataProtector 


} // End Namespace AspNetCore.DataProtector 
