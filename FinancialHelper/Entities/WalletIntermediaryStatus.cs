using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialHelper.Core.Entities
{
    public class WalletIntermediaryStatus
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal PriceBuy { get; set; }
        public decimal PriceStop { get; set; }
        public bool Buy { get; set; }
        public bool Sell { get; set; }

        //to string
        public override string ToString()
        {
            return $"{Symbol} - {Date} - Buy: {Buy} - Sell: {Sell} - PriceBuy: {PriceBuy} - PriceStop: {PriceStop}";
        }
    }
}
