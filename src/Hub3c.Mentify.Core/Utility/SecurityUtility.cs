using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Hub3c.Mentify.Core.Utility
{
    public class SecurityUtility
    {
        public const string RegexPassword = @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})";
        public static string EncryptString(string stringToEncrypt)
        {
            var encText = new UTF8Encoding();
            var btText = encText.GetBytes(stringToEncrypt);
            return Convert.ToBase64String(btText);
        }

        public static string DecryptString(string stringToDecrypt)
        {
            var bt64 = Convert.FromBase64String(stringToDecrypt);
            var decodedText = Encoding.UTF8.GetString(bt64);
            return decodedText;
        }

        public static bool CheckPasswordPattern(string password)
        {
            return Regex.IsMatch(password, RegexPassword);
        }

        public static string Encrypt(string plainText)
        {
            try
            {
                var key = GetKey();
                var cryptoProvider = Aes.Create();
                var memStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memStream,
                    cryptoProvider.CreateEncryptor(Encoding.ASCII.GetBytes(key[0]), Encoding.ASCII.GetBytes(key[1])),
                    CryptoStreamMode.Write);
                var writer = new StreamWriter(cryptoStream);
                writer.Write(plainText);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();

                var cipher = Convert.ToBase64String(memStream.ToArray(), 0, Convert.ToInt32(memStream.Length));
                return cipher.Replace('+', '!');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string Encrypt(int intValue)
        {
            return Encrypt(intValue.ToString(CultureInfo.InvariantCulture));
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                if ((cipherText != null))
                {
                    var key = GetKey();
                    var cryptoProvider = Aes.Create();
                    var memStream = new MemoryStream(Convert.FromBase64String(cipherText.Replace('!', '+')));
                    var cryptoStream = new CryptoStream(memStream, cryptoProvider.CreateDecryptor(Encoding.ASCII.GetBytes(key[0]), Encoding.ASCII.GetBytes(key[1])), CryptoStreamMode.Read);
                    var reader = new StreamReader(cryptoStream);
                    return reader.ReadToEnd();
                }
                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string[] GetKey()
        {
            try
            {
                var allKey = "C$4u#8Zxe4~9{4\\]3RdX!j0YI5%nS9Pd";
                var aesKey = string.Empty;
                var aesBlock = string.Empty;

                for (var i = 0; i <= allKey.Length - 1; i += 2)
                {
                    aesKey += allKey[i];
                    aesBlock += allKey[i + 1];
                }

                return new[]
                {
                    aesKey,
                    aesBlock
                };
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}