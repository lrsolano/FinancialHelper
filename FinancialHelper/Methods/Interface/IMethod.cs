using FinancialHelper.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.Core.Methods.Interface
{
    public interface IMethod
    {
        public List<WalletIntermediaryStatus> GetSignals();

        public void SetHistoricalCandles(IReadOnlyList<Candle> candles);

    }
}
