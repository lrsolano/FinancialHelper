using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialHelper.Core.Entities
{
    public class Wallet
    {
        public string Symbol { get; set; }
        public decimal Value { get; set; }
        public decimal StopLoss { get; set; }
        public decimal ValueApplicated { get; set; }
        public decimal Target { get; set; }
        public decimal TotalGain { get; set;}
        public long Gain { get; set; }
        public long Loss { get; set; }


        public Wallet(string symbol)
        {
            Symbol = symbol;
        }

        public void Buy(decimal value, decimal stopLoss, DateTime date)
        {
            ValueApplicated += value;
            StopLoss = stopLoss;
            Target = value * 1.1m;

            //Console.WriteLine($"Buy on {date:dd/MM/yyyy} value of {Math.Round(value,2)} and stop at {Math.Round(stopLoss,2)}");
        }

        public void Sell(decimal value, DateTime date)
        {

            var profit = value - ValueApplicated;

            ValueApplicated = 0;
            StopLoss = 0;
            Target = 0;

            if (profit > 0)
            {
                Gain++;
            }
            else
            {
                Loss++;
            }

            TotalGain += profit;

            //Console.WriteLine($"Sell on {date:dd/MM/yyyy} value of {Math.Round(value, 2)} profit {Math.Round(profit, 2)}");
            //Console.WriteLine();
        }

        public void UpdateStop(decimal value)
        {
            StopLoss = value;

            //Console.WriteLine($"Stop updated to {Math.Round(value, 2)}");
        }
    }
}
