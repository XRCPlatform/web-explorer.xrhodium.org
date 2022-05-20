using System.Collections.Generic;
using System.Linq;

namespace BitcoinRhExplorer.Server.Business
{
    public class PlaceItem
    {
        public string Currency { get; internal set; }
        public string Country { get; internal set; }
        public string CountryCode { get; internal set; }
        public string TwoLetterCountryCode { get; internal set; }
        public double Tax { get; internal set; }
        public double ConversionFromEuro { get; internal set; }
        public string CurrencySymbol { get; internal set; }

        public PlaceItem(string currency, string country, string countryCode, string twoLetterCountryCode, double tax, double conversionFromEuro, string currencySymbol)
        {
            Currency = currency;
            Country = country;
            CountryCode = countryCode;
            TwoLetterCountryCode = twoLetterCountryCode;
            Tax = tax;
            ConversionFromEuro = conversionFromEuro;
            CurrencySymbol = currencySymbol;
        }
    }

    public class PlaceComponent
    {
        public List<PlaceItem> Places { get; internal set; }

        public PlaceComponent()
        {
            Places = new List<PlaceItem>();
            Places.Add(new PlaceItem("CZK", "Czech Republic", "CZE", "CS", 0.21, 28, "Kč"));
        }

        public PlaceItem GetByTwoLetterCountryCode(string countryCode)
        {
            return Places.FirstOrDefault(p => p.TwoLetterCountryCode == countryCode);
        }

        public PlaceItem GetByCountryCode(string countryCode)
        {
            return Places.FirstOrDefault(p => p.CountryCode == countryCode);
        }

        public PlaceItem GetDefault()
        {
            return Places.First(p => p.CountryCode == "CZE");
        }
    }
}
