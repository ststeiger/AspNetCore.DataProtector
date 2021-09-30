/* ===============================================
* 功能描述：AspNetCore.DataProtection.IDataProtector
* 创 建 者：WeiGe
* 创建日期：9/12/2018 11:57:30 PM
* ===============================================*/



namespace AspNetCore.DataProtector
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataProtector
    {
        /// <summary>
        ///  Protect <paramref name="plainData"/> to byte[]
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        byte[] Protect(byte[] plainData);
        /// <summary>
        /// Protect <paramref name="plainText"/> to string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Protect(string plainText);
        /// <summary>
        /// Unprotect <paramref name="protectedData"/> to byte[]
        /// </summary>
        /// <param name="protectedData"></param>
        /// <returns></returns>
        byte[] Unprotect(byte[] protectedData);
        /// <summary>
        /// Unprotect <paramref name="protectedData"/> to string
        /// </summary>
        /// <param name="protectedData"></param>
        /// <returns></returns>
        string Unprotect(string protectedData);
        /// <summary>
        /// From <paramref name="base64Text"/> to byte[]
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        byte[] FromBase64(string base64Text);
        /// <summary>
        /// To string of <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        string ToBase64(byte[] bytes);
    }
}
