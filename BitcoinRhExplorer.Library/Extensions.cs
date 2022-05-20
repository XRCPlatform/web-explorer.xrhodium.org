using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace BitcoinRhExplorer.Library
{
    public static class Extensions
    {
        [DebuggerStepThrough]
        public static void IfDefined<T>(this T x, Action<T> f)
        {
            if (x != null)
            {
                f(x);
            }
        }

        [DebuggerStepThrough]
        public static TResult IfDefined<T, TResult>(this T x, Func<T, TResult> f, TResult otherwise = default(TResult))
        {
            if (x != null)
            {
                return f(x);
            }
            return otherwise;
        }

        [DebuggerStepThrough]
        public static bool IsAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        public static bool HasFile(this System.Web.HttpPostedFileBase file)
        {
            return file != null && file.ContentLength > 0;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static string AddNextText(this string text, string add)
        {
            return text + " " + add;
        }

        public static string AddWebName(this string text)
        {
            return text + " - " + Resources.Resources.WebTitle;
        }

        public static string GenerateUri(this string title, bool separatorBefore = true)
        {
            byte[] bytesUri = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(title);
            string tempTitle = System.Text.Encoding.ASCII.GetString(bytesUri).ToLower();

            tempTitle = Regex.Replace(tempTitle, @"[^a-z0-9\s-]", "");
            tempTitle = Regex.Replace(tempTitle, @"\s+", " ").Trim();
            tempTitle = tempTitle.Substring(0, tempTitle.Length <= 45 ? tempTitle.Length : 45).Trim();
            tempTitle = Regex.Replace(tempTitle, @"\s", "-");
            tempTitle = Regex.Replace(tempTitle, @"--+", "-");

            if (separatorBefore)
            {
                return "/" + tempTitle;
            }
            else
            {
                return tempTitle;                
            }
        }
    }
}
