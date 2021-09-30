
namespace AspNetCore.DataProtector
{


    public class TestRunner
    {


        public static string Test(string input)
        {
            AspNetCore.DataProtector.IDataProtector idp =
                new AspNetCore.DataProtector.DataProtector(
                      new AspNetCore.DataProtector.DataProtectorOptions()
                      {
                          AutoGenerateKeys = true,
                          UseCompress = false
                      }
            );

            byte[] prot = idp.Protect(System.Text.Encoding.UTF8.GetBytes(input));
            byte[] unprot = idp.Unprotect(prot);
            string output = System.Text.Encoding.UTF8.GetString(unprot);

            return output;
        }


    }


}
