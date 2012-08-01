using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SMCL.Utilities
{
    class Cryptography
    {
        public string EncryptSHA1(string text)
        {
            try
            {
                byte[] input = null;
                SHA1 crypt = SHA1Managed.Create();                                
                StringBuilder sb = new StringBuilder();
                ASCIIEncoding encoding = new ASCIIEncoding();

                input = crypt.ComputeHash(encoding.GetBytes(text));
                for (int i = 0; i < input.Length; i++)
                    sb.AppendFormat("{0:x2}", input[i]);

                return sb.ToString();


                /*UnicodeEncoding encoder = new UnicodeEncoding();

                byte[] input = encoder.GetBytes(text);
                byte[] output;

                SHA1 criptaSHA = new SHA1CryptoServiceProvider();

                output = criptaSHA.ComputeHash(input);

                StringBuilder sBuilder = new StringBuilder();

                //Format Bytes to Hex
                foreach (byte b in output)
                    sBuilder.Append(b.ToString("x2"));

                return sBuilder.ToString();*/
            }
            catch (ArgumentNullException ex)
            {
                throw new SystemException("Impossible to encrypt the string using SHA1: " + ex.Message);
            }
        }

        public string EncryptMD5(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                byte[] input = Encoding.ASCII.GetBytes(text);

                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                des.Key = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes("telerik"));

                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(input, 0, input.Length));
            }
            catch (ArgumentNullException ex)
            {
                throw new SystemException("Impossible to encrypt the string using MD5: " + ex.Message);
            }
        }

        public string DecryptMD5(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            try
            {
                byte[] output = Convert.FromBase64String(text);

                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                des.Key = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes("telerik"));

                return Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(output, 0, output.Length), 0, output.Length);
            }
            catch (ArgumentNullException ex)
            {
                throw new SystemException("Impossible to encrypt the string using MD5: " + ex.Message);
            }
        }
    }
}
