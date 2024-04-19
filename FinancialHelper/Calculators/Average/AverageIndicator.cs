using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Calculators.Average
{
    public class SimpleAverageIndicator : IAverageIndicator
    {
        public IReadOnlyList<Candle> Candles { get; set; }
        public int Days { get; set; }

        public SimpleAverageIndicator(int days)
        {
            Days = days;
        }

        public Dictionary<DateTime, decimal> GetHistoricalAverage(CandleField field)
        {
            Dictionary<DateTime, decimal> historicalMovingAverage = new Dictionary<DateTime, decimal>();

            if (Candles.Count < Days)
            {
                return historicalMovingAverage;
            }

            for (int i = 0; i < Candles.Count; i++)
            {
                if (i < Days)
                {
                    continue;
                }
                else
                {
                    decimal sum = 0;
                    for (int j = 0; j < Days; j++)
                    {
                        switch (field)
                        {
                            case CandleField.Open:
                                sum += Candles[i - j].Open;
                                break;
                            case CandleField.High:
                                sum += Candles[i - j].High;
                                break;
                            case CandleField.Low:
                                sum += Candles[i - j].Low;
                                break;
                            case CandleField.Close:
                                sum += Candles[i - j].Close;
                                break;
                        }
                    }
                    historicalMovingAverage.Add(Candles[i].DateTime, sum / Days);
                }
            }
            return historicalMovingAverage;
        }
    }
}
