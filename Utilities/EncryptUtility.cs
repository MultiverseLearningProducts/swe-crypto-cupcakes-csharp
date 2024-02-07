using System.Security.Cryptography;
using System.Text;

namespace SweCryptoCupcakesCsharp.Utilities;

public class EncryptUtility
{
    public readonly byte[] Key;
    public readonly byte[] IV;

    public EncryptUtility(IConfiguration configuration)
    {
        // Read secret keys through dependency injection - if none exist, set to default strings 
        Key = Encoding.UTF8.GetBytes(configuration["AES_KEY"] ?? "e0ecc8d25dd8ff8348339ceef19a9f81");
        IV = Encoding.UTF8.GetBytes(configuration["AES_IV"] ?? "32f3b2c782fb891a");
    }

    public string Encrypt(string plaintext)
    {
        byte[] encrypted;

        // Create an AesCryptoServiceProvider object with the specified key and IV.
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Create the streams used for encryption.
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        //Write all data to the stream.
                        sw.Write(plaintext);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);

    }

    public string Decrypt(string input)
    {
        // Declare string to hold decryptedText text
        string decryptedText = "";

        // Create an AesCryptoServiceProvider object with the specified key and IV.
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    
            // Create the streams used for decryption.
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(input)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        // Read the decrypted bytes from the decrypting stream and place them in a string.
                        decryptedText = sr.ReadToEnd();
                    }
                }
            }
        }
        return decryptedText;
    }
}