using System.Security.Cryptography;

namespace ManyInOneAPI.Infrastructure
{
    public static class LINQListShuffleExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int len = list.Count;
            while (len > 1)
            {
                len--;
                int k = GetCryptoRandomizeNumber(0, len + 1);
                // swap 
                T curr = list[k];
                list[k] = list[len];
                list[len] = curr;
            }
        }

        private static int GetCryptoRandomizeNumber(int min, int max)
        {
            using(var range = RandomNumberGenerator.Create()) 
            {
                byte[] randomNumber = new byte[4];
                range.GetBytes(randomNumber);
                int value = BitConverter.ToInt32(randomNumber, 0);
                return (Math.Abs(value) % (max - min)) + min;
            }
        }
    }
}
