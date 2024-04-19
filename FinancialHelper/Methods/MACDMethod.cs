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
    public class MACDMethod : IMethod
    {
        public string Symbol { get; set; }
        public IReadOnlyList<Candle> HistoricalCandles { get; set; }

        public IAverageIndicator SlowerAverage { get; set; }
        public IAverageIndicator FasterAverage { get; set; }

        public IAverageIndicator MoreFasterAverage { get; set; }

        public MACDMethod(IAverageIndicator faster, IAverageIndicator slower, IAverageIndicator moreFaster)
        {
            SlowerAverage = slower;
            FasterAverage = faster;
            MoreFasterAverage = moreFaster;
        }

        public List<WalletIntermediaryStatus> GetSignals()
        {
            List<WalletIntermediaryStatus> signals = new List<WalletIntermediaryStatus>();

            var fasterMovingAverage = FasterAverage.GetHistoricalAverage(CandleField.Close);
            var slowerMovingAverage = SlowerAverage.GetHistoricalAverage(CandleField.Close);
            
            Dictionary<DateTime,decimal> macd = GetMedianAverage(fasterMovingAverage, slowerMovingAverage);

            Dictionary<DateTime, decimal> signalLine = GetEma(macd, MoreFasterAverage.Days);

            var intermediarys = new List<GenericIntermediaryIndicator>();

            foreach(var signal in signalLine)
            {
                if (macd.ContainsKey(signal.Key))
                {
                    //check if exists the day before in both averages
                    if (!macd.ContainsKey(signal.Key.AddDays(-1)) || !signalLine.ContainsKey(signal.Key.AddDays(-1)))
                    {
                        continue;
                    }


                    var candle = HistoricalCandles.First(h => h.DateTime.Date == signal.Key.Date);

                    var hiloIntermediary = new GenericIntermediaryIndicator
                    {
                        Data = signal.Key,
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

                    var macdToday = macd[signal.Key];
                    var macdYesterday = macd[signal.Key.AddDays(-1)];

                    var signalToday = signal.Value;
                    var signalYesterday = signalLine[signal.Key.AddDays(-1)];


                    bool buyCross = signalToday > macdToday;
                    bool sellCross = signalToday < macdToday;

                    decimal max = (buyCross) ? candle.High : 0;
                    decimal min = (buyCross) ? candle.Low : 0;

                    decimal start = ((candle.Low < yesterday.MaxValue) && (candle.High > yesterday.MaxValue)) ? yesterday.MaxValue : 0;
                    decimal stop = ((candle.Low < yesterday.MaxValue) && (candle.High > yesterday.MaxValue)) ? previousCandle.Low : 0;

                    //create a new intermediary
                    hiloIntermediary.MaxValue = max;
                    hiloIntermediary.MinValue = min;
                    hiloIntermediary.Start = start;
                    hiloIntermediary.Stop = stop;
                    hiloIntermediary.Data = signal.Key;
                    hiloIntermediary.Average = macdToday;

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
            }

            return signals;

        }

        private Dictionary<DateTime, decimal> GetEma(Dictionary<DateTime, decimal> macd, int days)
        {
            
            Dictionary<DateTime, decimal> ema = new Dictionary<DateTime, decimal>();

            decimal multiplier = 2m / ((decimal)days + 1m);

            decimal sum = 0;
            
            foreach(var day in macd.Take(days))
            {
                sum += day.Value;
            }

            decimal initialEma = sum / days;

            ema.Add(macd.ElementAt(days).Key, initialEma);

            for(int i = days + 1; i < macd.Count; i++)
            {
                decimal value = macd.ElementAt(i).Value;

                decimal emaValue = (value - ema[macd.ElementAt(i - 1).Key]) * multiplier + ema[macd.ElementAt(i - 1).Key];

                ema.Add(macd.ElementAt(i).Key, emaValue);
            }

            return ema;
        }

        private Dictionary<DateTime, decimal> GetMedianAverage(Dictionary<DateTime, decimal> fasterMovingAverage, Dictionary<DateTime, decimal> slowerMovingAverage)
        {
            Dictionary<DateTime, decimal> median = new Dictionary<DateTime, decimal>();

            foreach(var day in slowerMovingAverage)
            {
                median.Add(day.Key, fasterMovingAverage[day.Key] - slowerMovingAverage[day.Key]);
            }

            return median;
        }

        public void SetHistoricalCandles(IReadOnlyList<Candle> candles)
        {
            HistoricalCandles = candles;
            SlowerAverage.Candles = candles;
            FasterAverage.Candles = candles;
            MoreFasterAverage.Candles = candles;
        }
    }
}
