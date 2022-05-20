using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace BitcoinRhExplorer.Library
{
    public static class HttpExtensions
    {
        public static bool IsSecured(this HttpRequest r)
        {
            return
                r.IsSecureConnection ||
                String.Equals(r.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsPost(this HttpRequestBase r)
        {
            return r.HttpMethod == "POST";
        }

        public static string GetString(this HttpRequestBase r, string key)
        {
            return r.Params[key] ?? string.Empty;
        }

        public static Guid? GetGuid(this HttpRequestBase r, string key, string alternativeKey = null)
        {
            Guid guid;
            if (!Guid.TryParse(r.GetString(key).Split(',').FirstOrDefault(), out guid))
            {
                if (alternativeKey != null)
                {
                    return r.GetGuid(alternativeKey);
                }
                return null;
            }
            return guid;
        }

        public static decimal? GetDecimal(this HttpRequestBase r, string key)
        {
            decimal result;
            if (!Decimal.TryParse(r.GetString(key), out result))
            {
                return null;
            }
            return result;
        }

        public static bool GetBool(this HttpRequestBase r, string key)
        {
            return (r.GetString(key) ?? "").ToLowerInvariant().Contains("true");
        }

        public static int? GetInt(this HttpRequestBase r, string key)
        {
            int result;
            if (!Int32.TryParse(r.GetString(key), out result))
            {
                return null;
            }
            return result;
        }

        public static long? GetLong(this HttpRequestBase r, string key)
        {
            long result;
            if (!Int64.TryParse(r.GetString(key), out result))
            {
                return null;
            }
            return result;
        }

        public static Dictionary<string, object> GetFormDictionary(this HttpRequestBase r)
        {
            var formDictionary = new Dictionary<string, object>();

            CopyNameValueCollection(r.QueryString, formDictionary);
            CopyNameValueCollection(r.Form, formDictionary);

            return formDictionary;
        }

        private static void CopyNameValueCollection(NameValueCollection collection, Dictionary<string, object> formDictionary)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var key = collection.GetKey(i);
                var values = collection.GetValues(i);
                formDictionary[key] = (values.Length > 1) ? (object)values.ToList() : values[0];
            }
        }
    }
}
