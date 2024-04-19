using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Calculators.Average
{
    public interface IAverageIndicator
    {
        public IReadOnlyList<Candle> Candles { get; set; }
        public int Days { get; set; }
        public Dictionary<DateTime, decimal> GetHistoricalAverage(CandleField field);
    }
}
