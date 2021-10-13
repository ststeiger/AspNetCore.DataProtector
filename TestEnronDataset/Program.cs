
namespace TestEnronDataset
{


    class Program
    {


        static async System.Threading.Tasks.Task Main(string[] args)
        {
            await EnronDataReader.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}

