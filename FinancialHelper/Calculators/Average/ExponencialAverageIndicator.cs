using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Calculators.Average
{
    public class ExponencialAverageIndicator : IAverageIndicator
    {
        public IReadOnlyList<Candle> Candles { get; set; }
        public int Days { get; set; }

        public ExponencialAverageIndicator(int days)
        {
            Days = days;
        }


        public Dictionary<DateTime, decimal> GetHistoricalAverage(CandleField field)
        {
            Dictionary<DateTime, decimal> historicalExponentialMovingAverage = new Dictionary<DateTime, decimal>();
            if (Candles.Count < Days)
            {
                return historicalExponentialMovingAverage;
            }

            // Calculate the multiplier
            decimal multiplier = 2m / ((decimal)Days + 1m);
            decimal sum = 0;
            for (int i = 0; i < Days; i++)
            {
                switch (field)
                {
                    case CandleField.Open:
                        sum += Candles[i].Open;
                        break;
                    case CandleField.High:
                        sum += Candles[i].High;
                        break;
                    case CandleField.Low:
                        sum += Candles[i].Low;
                        break;
                    case CandleField.Close:
                        sum += Candles[i].Close;
                        break;
                }
            }
            decimal initialEMA = sum / Days;
            historicalExponentialMovingAverage.Add(Candles[Days].DateTime, initialEMA);
            for (int i = Days + 1; i < Candles.Count; i++)
            {
                decimal value = 0;
                switch (field)
                {
                    case CandleField.Open:
                        value = Candles[i].Open;
                        break;
                    case CandleField.High:
                        value = Candles[i].High;
                        break;
                    case CandleField.Low:
                        value = Candles[i].Low;
                        break;
                    case CandleField.Close:
                        value = Candles[i].Close;
                        break;
                }

                decimal ema = (value - historicalExponentialMovingAverage[Candles[i - 1].DateTime]) * multiplier + historicalExponentialMovingAverage[Candles[i - 1].DateTime];

                historicalExponentialMovingAverage.Add(Candles[i].DateTime, ema);
            }
            return historicalExponentialMovingAverage;
        }
    }
}
