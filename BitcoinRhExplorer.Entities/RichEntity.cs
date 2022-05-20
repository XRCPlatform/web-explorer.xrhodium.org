using System;
using System.Globalization;
using System.Text;
using BitcoinRhExplorer.Library;

namespace BitcoinRhExplorer.Entities
{
    public abstract class RichEntity : IComparable, ICloneable, IIdEntity
    {
        public long Id { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public bool IsDeleted { get; set; }

        public virtual int CompareTo(object other)
        {
            return (other as RichEntity).IfDefined(e => -CreatedUtc.CompareTo(e.CreatedUtc));
        }

        public RichEntity()
        {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
            IsDeleted = false;
        }

        public new string GetHashCode()
        {
            using (var sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var textData = Encoding.UTF8.GetBytes(CreatedUtc.ToString(CultureInfo.InvariantCulture));

                var hash = sha1.ComputeHash(textData);

                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
