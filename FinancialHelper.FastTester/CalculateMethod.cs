using FinancialHelper.Core.Entities;
using FinancialHelper.Core.Methods.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancialHelper.FastTester
{
    public static class CalculateMethod
    {
        private static WalletResult Calculate(IMethod method, string symbol, IReadOnlyList<Candle> candles)
        {
            method.SetHistoricalCandles(candles);

            List<FinancialHelper.Core.Entities.WalletIntermediaryStatus> signals = method.GetSignals();

            long win = 0;
            long loss = 0;
            decimal totalWin = 0;

            Wallet wallet = new Wallet(symbol);

            foreach (Candle candle in candles)
            {
                if (signals.Any(s => s.Date == candle.DateTime))
                {
                    WalletIntermediaryStatus signal = signals.First(s => s.Date == candle.DateTime);


                    if (signal.Buy && wallet.ValueApplicated == 0)
                    {
                        wallet.Buy(candle.Close, signal.PriceStop, candle.DateTime);
                    }
                    if (signal.PriceStop > wallet.StopLoss && wallet.ValueApplicated > 0 && signal.Sell)
                    {
                        wallet.UpdateStop(signal.PriceStop);
                    }

                    if ((wallet.StopLoss > candle.Close && wallet.ValueApplicated > 0))
                    {
                        wallet.Sell(candle.Close, candle.DateTime);
                    }
                }

            }

            if (wallet.ValueApplicated > 0)
            {
                wallet.Sell(candles.Last().Close, candles.Last().DateTime);
            }

            if (wallet.TotalGain > 0)
            {
                win++;
            }
            else
            {
                loss++;
            }

            totalWin += wallet.TotalGain;



            WalletResult result = new WalletResult
            {
                Symbol = wallet.Symbol,
                ValueApplicated = wallet.ValueApplicated,
                TotalGain = wallet.TotalGain,
                Win = wallet.Gain,
                Losses = wallet.Loss,
                totalWin = totalWin
            };

            return result;
        }

        public async static Task<List<WalletResult>> Calculate(IMethod method, List<string> symbols, DateTime startDate, DateTime stopDate)
        {
            List<WalletResult> results = new List<WalletResult>();
            foreach (string symbol in symbols)
            {
                var candles = new List<Candle>();
                try
                {
                    candles = (List<Candle>)await Yahoo.GetHistoricalAsync(symbol, startDate, stopDate, Period.Daily);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in symbol {symbol}: {ex.Message}");
                    continue;
                }

                results.Add(Calculate(method, symbol, candles));
            }
            return results;
        }

        public async static Task<List<WalletResult>> CalculateTwoMethods(IMethod primeMethod, IMethod secondMethod, List<string> symbols, DateTime startDate, DateTime stopDate, bool bothOpen, bool bothClose)
        {
            List<WalletResult> results = new List<WalletResult>();
            foreach (string symbol in symbols)
            {
                var candles = new List<Candle>();
                try
                {
                    candles = (List<Candle>)await Yahoo.GetHistoricalAsync(symbol, startDate, stopDate, Period.Daily);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in symbol {symbol}: {ex.Message}");
                    continue;
                }

                WalletResult result = Calculate(primeMethod, secondMethod, symbol, candles, bothOpen, bothClose);
                results.Add(result);

            }
            return results;
        }

        private static WalletResult Calculate(IMethod primeMethod, IMethod secondMethod, string symbol, List<Candle> candles, bool bothOpen, bool bothClose)
        {
            primeMethod.SetHistoricalCandles(candles);
            secondMethod.SetHistoricalCandles(candles);

            List<FinancialHelper.Core.Entities.WalletIntermediaryStatus> signalsPrime = primeMethod.GetSignals();
            List<FinancialHelper.Core.Entities.WalletIntermediaryStatus> signalsSecond = secondMethod.GetSignals();

            long win = 0;
            long loss = 0;
            decimal totalWin = 0;

            Wallet wallet = new Wallet(symbol);

            foreach (Candle candle in candles)
            {
                if (signalsPrime.Any(s => s.Date == candle.DateTime) && signalsSecond.Any(s => s.Date == candle.DateTime))
                {
                    WalletIntermediaryStatus signalPrime = signalsPrime.First(s => s.Date == candle.DateTime);
                    WalletIntermediaryStatus signalSecond = signalsSecond.First(s => s.Date == candle.DateTime);

                    if(bothOpen)
                    {
                        if (signalPrime.Buy && signalSecond.Buy && wallet.ValueApplicated == 0)
                        {
                            wallet.Buy(candle.Close, signalPrime.PriceStop, candle.DateTime);
                        }
                    }
                    else
                    {
                        if ((signalPrime.Buy || signalSecond.Buy ) && wallet.ValueApplicated == 0)
                        {
                            wallet.Buy(candle.Close, signalPrime.PriceStop, candle.DateTime);
                        }
                    }

                    if (bothClose)
                    {
                        if ((signalPrime.PriceStop > wallet.StopLoss && wallet.ValueApplicated > 0 && signalPrime.Sell) || (signalSecond.PriceStop > wallet.StopLoss && wallet.ValueApplicated > 0 && signalSecond.Sell))
                        {
                            wallet.UpdateStop(signalPrime.PriceStop);
                        }
                    }
                    else
                    {
                        if (signalPrime.PriceStop > wallet.StopLoss && wallet.ValueApplicated > 0 && signalPrime.Sell)
                        {
                            wallet.UpdateStop(signalPrime.PriceStop);
                        }
                    }

                    if ((wallet.StopLoss > candle.Close && wallet.ValueApplicated > 0))
                    {
                        wallet.Sell(candle.Close, candle.DateTime);
                    }
                }

            }

            if (wallet.ValueApplicated > 0)
            {
                wallet.Sell(candles.Last().Close, candles.Last().DateTime);
            }

            if (wallet.TotalGain > 0)
            {
                win++;
            }
            else
            {
                loss++;
            }

            totalWin += wallet.TotalGain;



            WalletResult result = new WalletResult
            {
                Symbol = wallet.Symbol,
                ValueApplicated = wallet.ValueApplicated,
                TotalGain = wallet.TotalGain,
                Win = wallet.Gain,
                Losses = wallet.Loss,
                totalWin = totalWin
            };

            return result;
        }
    }
}
