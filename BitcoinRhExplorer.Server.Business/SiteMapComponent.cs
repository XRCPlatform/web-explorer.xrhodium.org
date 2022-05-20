using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinRhExplorer.Server.Business
{
    public class SiteMapComponent
    {
        public enum ChangeFrequency
        {
            Always,
            Hourly,
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Never
        }

        public interface ISitemapItem
        {
            string Url { get; }
            DateTime? LastModified { get; }
            ChangeFrequency? ChangeFrequency { get; }
            double? Priority { get; }
        }

        public class SitemapItem : ISitemapItem
        {
            public SitemapItem(string url)
            {
                Url = url;
            }

            public string Url { get; set; }

            public DateTime? LastModified { get; set; }

            public ChangeFrequency? ChangeFrequency { get; set; }

            public double? Priority { get; set; }
        }
    }
}
