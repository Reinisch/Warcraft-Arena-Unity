using System;
using System.Collections.Generic;
using System.Text;

namespace ElleFramework.Utils
{
    public class TextUtils
    {
        /// <summary>
        /// Determines whether a string is null or contains only spaces.
        /// </summary>
        /// <param name="str">String to analyze</param>
        /// <returns>True if the string is not null and contains at least one character other than a space. False otherwise.</returns>
        public static bool IsEmpty(string str)
        {
            return ((str == null) || str.Trim().Equals(""));
        }

        public static bool HasValue(string str)
        {
            return !IsEmpty(str);
        }

        /// <summary>
        /// Cut the string passed to maxLen characters at maximum, adding the ellipses in the event of a cut
        /// </summary>
        /// <param name="str">string to cut</param>
        /// <param name="maxLen">max length</param>
        /// <returns>string to display</returns>
        public static string Truncate(string str, int maxLen)
        {
            if (str == null)
                str = "";
            else
            {

                maxLen = Math.Max(4, maxLen);

                if (str.Length > maxLen)
                    str = str.Substring(0, maxLen - 3) + "...";
            }

            return (str);
        }

        /// <summary>
        /// Escapes a string so that it is represented correctly in an HTML page.
        /// Even the "new lines" are maintained
        /// </summary>
        /// <param name="strToConvert">String to translate in HTML</param>
        /// <returns>String converted in HTML</returns>
        public static string HTMLEscape(string strToConvert)
        {
            StringBuilder str = new StringBuilder();
            HTMLEntities ent = new HTMLEntities();
            string rval;

            for (int i = 0; i < strToConvert.Length; i++)
                str.Append(ent.GetEntity(strToConvert.Substring(i, 1)));

            rval = str.ToString().Replace("\n", "<br>");

            return (rval);
        }

        /// <summary>
        /// Calculates a HashCode of the string passed as an argument
        /// </summary>
        /// <param name="str">String to calculate the Hash</param>
        /// <returns>HashCode of the string</returns>
        public static int GetHashCode(string str)
        {
            int ret = 0;

            foreach (char c in str)
                ret = c + (ret << 6) + (ret << 16) - ret;

            return ret;
        }

        public static string ReplaceText(string structureWithTags,
                                              Dictionary<string, string> keyValuePair,
                                              string startTag,
                                              string endTag)
        {
            string retString = structureWithTags ?? "";

            foreach (KeyValuePair<string, string> dictVal in keyValuePair)
                retString = retString.Replace(startTag + dictVal.Key + endTag, dictVal.Value);
            return retString;
        }

    }
}
