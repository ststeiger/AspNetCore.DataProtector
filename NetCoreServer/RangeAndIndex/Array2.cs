
namespace NetCoreServer
{


    internal class Array2
    {


        public static void Fill<T>(T[] array, T value)
        {
            if (array == null)
            {
                throw new System.ArgumentNullException(nameof(array));
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }


    }


}
