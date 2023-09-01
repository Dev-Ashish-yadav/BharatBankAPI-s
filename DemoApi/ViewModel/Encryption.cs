using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System;

namespace DemoApi.ViewModel
{
    public class Encryption
    { }
    [Serializable]
    public class JsonEncryptedData
    {
        private byte[] saltBytes;
        private string TOKEN;

        public JsonEncryptedData()
        {
            this.saltBytes = GetSalt();
            this.TOKEN = "TOKEN TO BE SHARED BY DGFT";
        }

        private byte[] GetSalt()
        {
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[16];
                random.GetBytes(bytes);
                return bytes;
            }
        }

        public string GetEncryptedData(string data)
        {
            try
            {
                Console.WriteLine("GetEncryptedData starts");
                string output = null;
                Console.WriteLine("data " + data);
                JsonRequestData reqData = new JsonRequestData();
                string encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
                reqData.Data = JsonEncryption(encodedData);
                reqData.Sign = JsonSign(encodedData);
                string json = JsonConvert.SerializeObject(reqData);
                output = json;
                Console.WriteLine("output " + output);
                Console.WriteLine("GetEncryptedData ends");
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                throw e;
            }
        }

        private string JsonSign(string data)
        {
            Console.WriteLine("JsonSign starts");
            string signedData = null;
            try
            {
                X509Certificate2 cert = new X509Certificate2("Bank.pfx", "certificatePassword");
                using (RSA rsa = cert.GetRSAPrivateKey())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    byte[] signature = rsa.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    signedData = Convert.ToBase64String(signature);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                throw e;
            }
            Console.WriteLine("JsonSign ends");
            return signedData;
        }

        public string JsonEncryption(string data)
        {
            Console.WriteLine("JsonEncryption starts");
            string encryptedData = null;
            try
            {
                using (Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(TOKEN, saltBytes, 65536))
                {
                    byte[] key = keyDerivation.GetBytes(256 / 8);
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = key;
                        aesAlg.GenerateIV();
                        byte[] iv = aesAlg.IV;
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            outputStream.Write(saltBytes, 0, saltBytes.Length);
                            outputStream.Write(iv, 0, iv.Length);
                            using (CryptoStream cryptoStream = new CryptoStream(outputStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                byte[] bytes = Encoding.UTF8.GetBytes(data);
                                cryptoStream.Write(bytes, 0, bytes.Length);
                                cryptoStream.FlushFinalBlock();
                                encryptedData = Convert.ToBase64String(outputStream.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                throw e;
            }
            Console.WriteLine("JsonEncryption ends");
            return encryptedData;
        }
    }

    [Serializable]
    public class JsonRequestData
    {
        public string Data { get; set; }
        public string Sign { get; set; }
    }

}
