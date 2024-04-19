using FinancialHelper.Core.Calculators.Average;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Calculators
{
    public class HiLoIndicator
    {
        public int Window { get; set; }

        public IReadOnlyList<Candle> Candles { get; set; }

        public HiLoIndicator(int window, IReadOnlyList<Candle> candles)
        {
            Window = window;
            Candles = candles;
        }

        public Dictionary<DateTime, int> GetSinalHiLo()
        {
            Dictionary<DateTime, int> sinalHiLo = new Dictionary<DateTime, int>();

            SimpleAverageIndicator simpleAverageIndicator = new SimpleAverageIndicator(Window);
            simpleAverageIndicator.Candles = Candles;

            var highSimpleAverage = simpleAverageIndicator.GetHistoricalAverage(CandleField.High);

            var lowSimpleAverage = simpleAverageIndicator.GetHistoricalAverage(CandleField.Low);

            for(int i = 0; i < lowSimpleAverage.Count; i++)
            {
                Candle candle = Candles[i + Window];

                long entrada = (candle.Close > highSimpleAverage[candle.DateTime])? 1 : 0;

                long saida = (candle.Low < lowSimpleAverage[candle.DateTime])? 1 : 0;

                sinalHiLo.Add(candle.DateTime, (int)(entrada - saida));

            }

            return sinalHiLo;
        }
    }
}
