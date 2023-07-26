using System.Runtime.CompilerServices;

namespace ChainResource
{
    public class ExchangeRateList
    {
        public Dictionary<string, decimal> Rates { get; set; }
        public long Timestamp { get; set; }
        public string Base { get; set; }

        public ExchangeRateList()
        {
            Rates = new Dictionary<string, decimal>();
            Timestamp = 0;
            Base = "";
        }

        public ExchangeRateList(Dictionary<string, decimal> rates, long timestamp, string baseCurrency)
        {
            Rates = rates;
            Timestamp = timestamp;
            Base = baseCurrency;
        }

        public override string ToString()
        {
            string res = "";
            res += $"timestamp: {Timestamp} \n";
            res += $"base: {Base} \n";
            res += string.Join(Environment.NewLine, Rates);

            return res;
        }
    }
}