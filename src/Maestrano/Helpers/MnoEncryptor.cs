using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Helpers
{
    public class MnoEncryptor
    {
        public static String encrypt(string str, string symKey)
        {
            RijndaelManaged AesEncryption = new RijndaelManaged();

            // Configure Encryptor
            AesEncryption.KeySize = 256; // 192, 256
            AesEncryption.BlockSize = 128;
            AesEncryption.Mode = CipherMode.CBC;
            AesEncryption.Padding = PaddingMode.PKCS7;

            // Pass key
            byte[] pwdBytes = Encoding.ASCII.GetBytes(symKey);
            byte[] keyBytes = new byte[32];
            int len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            AesEncryption.Key = keyBytes;

            // This array will contain the plain text in bytes
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(str);

            // Creates Symmetric encryption object   
            ICryptoTransform crypto = AesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(AesEncryption.IV) + "--" + Convert.ToBase64String(cipherText);
        }
    }
}
