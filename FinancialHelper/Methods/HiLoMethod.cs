using FinancialHelper.Core.Calculators;
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
    public class HiLoMethod : IMethod
    {
        public string Symbol { get; set; }
        public IReadOnlyList<Candle> HistoricalCandles { get; set; }
        public int Window { get; set; }
        public IAverageIndicator AverageIndicator { get; set; }

        public HiLoMethod(int window, IAverageIndicator averageIndicators)
        {
            Window = window;
            AverageIndicator = averageIndicators;
        }

        public List<WalletIntermediaryStatus> GetSignals()
        {

            var signals = new List<WalletIntermediaryStatus>();

            var hiloIntermediarys = new List<GenericIntermediaryIndicator>();

            var average = AverageIndicator.GetHistoricalAverage(CandleField.Close);

            var hiloIndicator = new HiLoIndicator(Window, HistoricalCandles);

            var signalHiLo = hiloIndicator.GetSinalHiLo();

            foreach (var candle in HistoricalCandles)
            {
                if (!signalHiLo.ContainsKey(candle.DateTime)) continue;
                if (!average.ContainsKey(candle.DateTime) || !average.ContainsKey(candle.DateTime.AddDays(-1)) || !average.ContainsKey(candle.DateTime.AddDays(-2))) continue;

                var data = candle.DateTime;

                var hiloIntermediary = new GenericIntermediaryIndicator
                {
                    Data = data,
                    MaxValue = 0,
                    MinValue = 0,
                    Start = 0,
                    Stop = 0
                };

                var averageAtualDay = average[candle.DateTime];

                var yesterday = hiloIntermediarys.FirstOrDefault(h => h.Data == candle.DateTime.AddDays(-1));

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

                var hiloAtualDay = signalHiLo[candle.DateTime];

                var maxValue = ((averageAtualDay < candle.High) && (averageAtualDay > candle.Low) && averageAtualDay > yesterday.Average) ? candle.High : 0;

                var minValue = ((averageAtualDay < candle.High) && (averageAtualDay > candle.Low) && averageAtualDay < yesterday.Average) ? candle.Low : 0;

                var start = ((candle.High > yesterday.MaxValue) && (hiloAtualDay > 0)) ? yesterday.MaxValue : 0;

                var stop = ((candle.High > yesterday.MaxValue)) ? previousCandle.Low : 0;

                //create a new intermediary
                hiloIntermediary.MaxValue = maxValue;
                hiloIntermediary.MinValue = minValue;
                hiloIntermediary.Start = start;
                hiloIntermediary.Stop = stop;
                hiloIntermediary.Data = data;
                hiloIntermediary.Average = averageAtualDay;

                hiloIntermediarys.Add(hiloIntermediary);


                signals.Add(new WalletIntermediaryStatus
                {
                    Date = candle.DateTime,
                    Symbol = Symbol,
                    Buy = (start > 0 && candle.Close > start),
                    Sell = (stop > 0),
                    PriceBuy = candle.Close,
                    PriceStop = stop - 0.05m
                }) ;

            }

            return signals;
        }

        public void SetHistoricalCandles(IReadOnlyList<Candle> candles)
        {
           
            HistoricalCandles = candles;
            AverageIndicator.Candles = candles;
        }
    }
}