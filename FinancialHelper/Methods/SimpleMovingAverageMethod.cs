using FinancialHelper.Core.Calculators.Average;
using FinancialHelper.Core.Entities;
using FinancialHelper.Core.Methods.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Methods
{
    public class SimpleMovingAverageMethod : IMethod
    {
        public string Symbol { get; set; }
        public IReadOnlyList<Candle> HistoricalCandles { get; set; }

        public IAverageIndicator SlowerAverage { get; set; }


        public SimpleMovingAverageMethod(IAverageIndicator slower)
        {
            SlowerAverage = slower;
        }

        public List<WalletIntermediaryStatus> GetSignals()
        {
            List<WalletIntermediaryStatus> signals = new List<WalletIntermediaryStatus>();

            var slowerMovingAverage = SlowerAverage.GetHistoricalAverage(CandleField.Close);

            var intermediarys = new List<GenericIntermediaryIndicator>();

            foreach (var slowerPair in slowerMovingAverage)
            {

                //skip if the day before does not exist
                if (!slowerMovingAverage.ContainsKey(slowerPair.Key.AddDays(-1)))
                {
                    continue;
                }

                var candle = HistoricalCandles.First(h => h.DateTime.Date == slowerPair.Key.Date);

                var hiloIntermediary = new GenericIntermediaryIndicator
                {
                    Data = slowerPair.Key,
                    MaxValue = 0,
                    MinValue = 0,
                    Start = 0,
                    Stop = 0
                };

                var yesterday = intermediarys.FirstOrDefault(h => h.Data == candle.DateTime.AddDays(-1));

                if (yesterday == null)
                {
                    yesterday = new GenericIntermediaryIndicator
                    {
                        Data = candle.DateTime.AddDays(-1),
                        MaxValue = 0,
                        MinValue = 0,
                        Start = 0,
                        Stop = 0
                    };
                }

                var previousCandle = HistoricalCandles.FirstOrDefault(c => c.DateTime == candle.DateTime.AddDays(-1));


                var slowerToday = slowerPair.Value;
                var slowerYesterday = slowerMovingAverage[slowerPair.Key.AddDays(-1)];


                bool buyCross = slowerToday > slowerYesterday;


                decimal max = ((slowerToday < candle.High) && (slowerToday > candle.Low) && buyCross) ? candle.High : 0;
                decimal min = ((slowerToday < candle.High) && (slowerToday > candle.Low) && buyCross) ? candle.Low : 0;

                decimal start = ((candle.Low < yesterday.MaxValue) && (candle.High > yesterday.MaxValue)) ? yesterday.MaxValue : 0;
                decimal stop = ((candle.Low < yesterday.MaxValue) && (candle.High > yesterday.MaxValue)) ? previousCandle.Low : 0;

                //create a new intermediary
                hiloIntermediary.MaxValue = max;
                hiloIntermediary.MinValue = min;
                hiloIntermediary.Start = start;
                hiloIntermediary.Stop = stop;
                hiloIntermediary.Data = slowerPair.Key;
                hiloIntermediary.Average = slowerToday;

                intermediarys.Add(hiloIntermediary);

                signals.Add(new WalletIntermediaryStatus
                {
                    Date = candle.DateTime,
                    Symbol = Symbol,
                    Buy = (start > 0 && candle.Close > start),
                    Sell = (stop > 0),
                    PriceBuy = candle.Close,
                    PriceStop = stop - 0.05m
                });

            }



            return signals;
        }

        public void SetHistoricalCandles(IReadOnlyList<Candle> candles)
        {
            HistoricalCandles = candles;
            SlowerAverage.Candles = candles;
        }
    }
}
