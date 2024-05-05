using ManyInOneAPI.Constants;
using System.Security.Cryptography;
using System.Text;

namespace ManyInOneAPI.Infrastructure.Encryption
{
    public static class AESOperation
    {
        public static string Encrypt(string normalText)
        {
            byte[] iv = new byte[16];
            byte[] byteArr;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AppConstant.EncryptionKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(normalText);

                        }
                        byteArr = ms.ToArray();

                        return Convert.ToBase64String(byteArr);
                    }
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(encryptedText);

            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(AppConstant.EncryptionKey);
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            MemoryStream ms = new MemoryStream(buffer);

            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            StreamReader sw = new StreamReader(cs);

            return sw.ReadToEnd();
        }
    }
}
