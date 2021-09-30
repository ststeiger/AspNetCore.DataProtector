/* ===============================================
* 功能描述：AspNetCore.DataProtection.DataProtectionOptions
* 创 建 者：WeiGe
* 创建日期：9/12/2018 11:02:57 PM
* ===============================================*/

using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.DataProtector
{
   
    public class DataProtectorOptions
    {
        private const string DataProtectionKeysFolderName = "ASP.NET/DataProtection-Keys";
        /// <summary>
        /// Compress the data
        /// </summary>
        public bool UseCompress { get; set; }
        /// <summary>
        /// default 'false'
        /// </summary>
        public bool AutoGenerateKeys { get; set; } = false;
        /// <summary>
        /// default 30 days
        /// </summary>
        public TimeSpan KeyExpired { get; set; } = TimeSpan.FromDays(30);

        static DirectoryInfo _keyPath;
        internal DirectoryInfo KeyDirectory
        {
            get
            {
                if (_keyPath == null)
                {
                    var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                    if (!string.IsNullOrEmpty(userProfile))
                    {
                        userProfile = Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "Users",
                        Environment.GetEnvironmentVariable("APP_POOL_ID")?? Environment.GetEnvironmentVariable("UserName"),
                        "AppData\\Local");
                    }
                    List<string> keyPaths = new List<string>(5)
                    {
                        Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                        userProfile,
                        Environment.GetEnvironmentVariable("HOME"),
                        AppContext.BaseDirectory
                    };
                    foreach (var path in keyPaths)
                    {
                        if (CreateDirectory(path, ref _keyPath))
                        {
                            break;
                        }
                    }
                }
                return _keyPath;
            }
        }
        private static bool CreateDirectory(string basePath, ref DirectoryInfo _keyPath)
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                try
                {                   
                    var retVal = new DirectoryInfo(Path.Combine(basePath, DataProtectionKeysFolderName));
                    if (!retVal.Exists)
                    {
                        retVal.Create();
                    }
                    _keyPath = retVal;
                    return true;
                }
                catch
                { 
                }               
            }
            return false;
        }
    }
}
