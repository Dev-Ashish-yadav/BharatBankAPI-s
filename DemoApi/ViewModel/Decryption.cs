using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System;

namespace DemoApi.ViewModel
{
    public class Decryption
    {
    }
    [Serializable]
    public class JsonDecryptedData
    {
        private static string TOKEN = "TOKEN TO BE SHARED BY DGFT";

        public string GetDecryptedData(string data)
        {
            try
            {
                Console.WriteLine("GetDecryptedData starts");
                string output = null;
                Console.WriteLine("data " + data);
                //Gson gson = new Gson();
                JsonRequestDataD requestRead = JsonConvert.DeserializeObject<JsonRequestDataD>(data);
                string decryptedData = JsonDecryption(requestRead.Data);
                bool isSignValid = JsonSignVerify(decryptedData, requestRead.Sign);
                Console.WriteLine("isSignValid " + isSignValid);
                if (isSignValid)
                {
                    output = Encoding.UTF8.GetString(Convert.FromBase64String(decryptedData));
                    Console.WriteLine("output " + output);
                }
                else
                {
                    throw new Exception();
                }
                Console.WriteLine("GetDecryptedData ends");
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                throw e;
            }
        }

        private bool JsonSignVerify(string data, string sign)
        {
            Console.WriteLine("JsonSignVerify starts");
            bool isSignValid = false;
            try
            {
                X509Certificate2 cert = new X509Certificate2("PublicJKS.cer");
                using (RSA rsa = cert.GetRSAPublicKey())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    isSignValid = rsa.VerifyData(bytes, Convert.FromBase64String(sign), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                throw ex;
            }
            Console.WriteLine("JsonSignVerify ends");
            return isSignValid;
        }

        public string JsonDecryption(string data)
        {
            Console.WriteLine("JsonDecryption starts");
            string decryptedData = null;
            try
            {
                byte[] cipherText = Convert.FromBase64String(data);
                if (cipherText.Length < 48)
                {
                    return null;
                }
                byte[] salt = cipherText[..16];
                byte[] iv = cipherText[16..32];
                byte[] ct = cipherText[32..];
                using (Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(TOKEN, salt, 65536))
                {
                    byte[] key = keyDerivation.GetBytes(256 / 8);
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = key;
                        aesAlg.IV = iv;
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(outputStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(ct, 0, ct.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedData = Encoding.UTF8.GetString(outputStream.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                throw ex;
            }
            Console.WriteLine("JsonDecryption ends");
            return decryptedData;
        }
    }

    
    public class JsonRequestDataD
    {
        public string Data { get; set; }
        public string Sign { get; set; }
    }
}
