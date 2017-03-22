namespace test
{
    public class Helper
    {
        public static void Swap<T>(T[] array2, int i, int j)
        {
            var temp = array2[i];
            array2[i] = array2[j];
            array2[j] = temp;
        }
    }
}
