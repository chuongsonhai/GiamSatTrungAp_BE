using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core
{
    public static class CommonUtils
    {
        public static int TotalDate(DateTime fromTime, DateTime toTime)
        {
            if (fromTime.Date == toTime.Date) return 1;
            int total = 0;
            if (toTime.Date > fromTime.Date)
            {
                DateTime endDate = toTime.Date;
                DateTime startDate = fromTime.Date;
                while (startDate < endDate.AddDays(1))
                {
                    DateTime currenttime = startDate;
                    if (currenttime <= endDate)
                    {
                        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(currenttime);
                        if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
                            total++;
                    }
                    startDate = startDate.AddDays(1);
                }
            }
            return total;
        }

        public static DateTime GetDate(int year, int quater, out DateTime totime)
        {
            DateTime firstDate = new DateTime();
            totime = new DateTime();
            switch (quater)
            {
                case 1:
                    firstDate = new DateTime(year, 01, 01);
                    totime = new DateTime(year, 03, 31);
                    return firstDate;
                case 2:
                    firstDate = new DateTime(year, 04, 01);
                    totime = new DateTime(year, 06, 30);
                    return firstDate;
                case 3:
                    firstDate = new DateTime(year, 07, 01);
                    totime = new DateTime(year, 09, 30);
                    return firstDate;
                default:
                    firstDate = new DateTime(year, 10, 01);
                    totime = new DateTime(year, 12, 31);
                    return firstDate;
            }
        }

        static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }            
            return builder.ToString();
        }

        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static string RandomValue(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(2));
            for (int i = 0; i < size - 3; i++)
            {
                builder.Append(RandomNumber(i, 9));
            }
            builder.Append(RandomString(1));
            return builder.ToString();
        }

        public static string GenerateSalt()
        {
            byte[] buf = new byte[SALT_SIZE];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public static string EncodePassword(string pass, string salt)
        {
            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bRet = null;

            HashAlgorithm hm = GetHashAlgorithm();
            if (hm is KeyedHashAlgorithm)
            {
                KeyedHashAlgorithm kha = (KeyedHashAlgorithm)hm;
                if (kha.Key.Length == bSalt.Length)
                {
                    kha.Key = bSalt;
                }
                else if (kha.Key.Length < bSalt.Length)
                {
                    byte[] bKey = new byte[kha.Key.Length];
                    Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
                    kha.Key = bKey;
                }
                else
                {
                    byte[] bKey = new byte[kha.Key.Length];
                    for (int iter = 0; iter < bKey.Length;)
                    {
                        int len = Math.Min(bSalt.Length, bKey.Length - iter);
                        Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
                        iter += len;
                    }
                    kha.Key = bKey;
                }
                bRet = kha.ComputeHash(bIn);
            }
            else
            {
                byte[] bAll = new byte[bSalt.Length + bIn.Length];
                Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                bRet = hm.ComputeHash(bAll);
            }

            return Convert.ToBase64String(bRet);
        }

        static HashAlgorithm GetHashAlgorithm()
        {
            HashAlgorithm hashAlgo = HashAlgorithm.Create(s_HashAlgorithm);
            if (hashAlgo == null)
                return null;
            return hashAlgo;
        }

        internal const int SALT_SIZE = 16;

        static string s_HashAlgorithm = "SHA256";
    }
}
