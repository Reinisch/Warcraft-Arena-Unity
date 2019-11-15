using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElleRealTime.Shared.BO
{
    public static class Utils
    {
        public static DB ElleRealTimeDB { get; set; }
        public static string PassPhrase = "SEIFINOCCHIO!";
        public static string CurrentLang { get; set; }

        public static string Environment { get; set; }

        public static string GenerateHashPassword(string username, string password)
        {
            username = username.ToUpper();
            password = password.ToUpper();

            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            string hashedpsw = Utils.HexStringFromBytes(bytehash).ToUpper();

            return hashedpsw;
        }
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for (int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if (s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }

        public static void CopyPropertiesTo<T, TU>(this T source,
            TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    {
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }

            }

        }

    }

}
