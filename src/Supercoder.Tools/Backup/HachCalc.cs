using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Supercoder.Tools
{
    public class HashCalc
    {
        public static string getSHA1(string s)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.Default.GetBytes(s))).Replace("-", "");
        }

        public static string ComputeMD5Hash(string fileName)
        {
            return ComputeHash(fileName, new MD5CryptoServiceProvider());
        }

        public static string ComputeSHA1Hash(string fileName)
        {
            return ComputeHash(fileName, new SHA1CryptoServiceProvider());
        }

        public static string ComputeHash(string fileName, HashAlgorithm hashAlgorithm)
        {
            FileStream stmcheck = File.OpenRead(fileName);
            try
            {
                byte[] hash = hashAlgorithm.ComputeHash(stmcheck);
                string computed = BitConverter.ToString(hash).Replace("-", "");
                return computed;
            }
            finally
            {
                stmcheck.Close();
            }
        }
    }

    public class TimedHashCalc
    {
        private DateTime startTime;
        private DateTime stopTime;

        public TimeSpan Elapsed ()
        {
            TimeSpan duration = stopTime - startTime;
            return (duration);
        }

        public string ComputeMD5Hash(string fileName)
        {
            return ComputeHash(fileName, new MD5CryptoServiceProvider());
        }

        public string ComputeSHA1Hash(string fileName)
        {
            return ComputeHash(fileName, new SHA1CryptoServiceProvider());
        }

        public string ComputeHash(string fileName, HashAlgorithm
        hashAlgorithm)
        {
            startTime = DateTime.Now;

            FileStream stmcheck = File.OpenRead(fileName);
            try
            {
                byte[] hash = hashAlgorithm.ComputeHash(stmcheck);
                string computed = BitConverter.ToString(hash).Replace("-", "");

                stopTime = DateTime.Now;

                return computed;
            }
            finally
            {
                stmcheck.Close();
            }
        }
    }
}
