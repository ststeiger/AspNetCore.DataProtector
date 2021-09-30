/* ===============================================
* 功能描述：AspNetCore.DataProtection.DataProtectionServiceExtensions
* 创 建 者：WeiGe
* 创建日期：9/13/2018 12:09:18 AM
* ===============================================*/

using AspNetCore.DataProtector;
using Microsoft.Extensions.Options;
using System;


namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataProtectorServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataProtector(this IServiceCollection services, Action<DataProtectorOptions> action = null)
        {
            services.AddSingleton<IDataProtector, DataProtector>();
            services.AddOptions<IOptions<DataProtectorOptions>>();
            services.Configure<DataProtectorOptions>(x => action?.Invoke(x));
            return services;
        }
    }
}
