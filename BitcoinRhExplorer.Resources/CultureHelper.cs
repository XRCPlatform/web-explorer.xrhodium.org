using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace BitcoinRhExplorer.Resources
{
    public static class CultureHelper
    {
        private static readonly IEnumerable<string> ValidCultures = new List<string>
        {
            "cs",
            "cs-CZ",
            "en-GB",
            "en-US",
            "sk",
            "sk-SK"
        };

        public enum CulturesAbbreviation
        {
            en = 0,
            cs = 1,
            sk = 2
        }

        private static readonly IEnumerable<string> Cultures = new List<string>
        {
            CulturesAbbreviation.en.ToString(), //default
            CulturesAbbreviation.cs.ToString(),
            CulturesAbbreviation.sk.ToString()
        };

        public static bool IsRighToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }

        public static string GetImplementedCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetDefaultCulture();
            }

            if (ValidCultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
            {
                return GetDefaultCulture();
            }

            if (Cultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
            {
                return name;
            }

            var n = GetNeutralCulture(name);

            foreach (var c in Cultures)
            {
                if (c.StartsWith(n))
                {
                    return c;
                }
            }

            return GetDefaultCulture();
        }

        public static string GetDefaultCulture()
        {
            return Cultures.First();
        }

        public static CultureInfo GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }

        public static string GetNeutralCulture(string name)
        {
            if (name.Length < 2)
            {
                return name;
            }

            return name.Substring(0, 2);
        }

        public static IEnumerable<string> GetCultures()
        {
            return Cultures;
        }
    }
}
