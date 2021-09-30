
namespace NetCoreServer
{
    /// <summary> Useful in number of places that return an empty byte array to avoid unnecessary memory allocation. </summary>
    public static class Array2<T>
    {


        public static T[] Empty()
        {
            return new T[0];
        }


    }

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
