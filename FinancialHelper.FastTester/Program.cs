// See https://aka.ms/new-console-template for more information
using FinancialHelper.Core.Calculators.Average;
using FinancialHelper.Core.Entities;
using FinancialHelper.Core.Methods;
using FinancialHelper.Core.Methods.Interface;
using FinancialHelper.FastTester;
using YahooFinanceApi;

//list of symbols
var symbols = new List<string> {  "WEGE3.SA", "CSMG3.SA", "ALOS3.SA",  "ECOR3.SA", "PRIO3.SA", "CVCB3.SA", "EGIE3.SA", "FLRY3.SA", "TOTS3.SA" };



var initialDate = new DateTime(2021, 1, 1);
var finalDate = DateTime.Now;


IAverageIndicator exponencialSlower = new ExponencialAverageIndicator(26);
IAverageIndicator exponencialFast = new ExponencialAverageIndicator(12);
IAverageIndicator exponencialMoreFaster = new ExponencialAverageIndicator(9);


IAverageIndicator hiloSLow = new ExponencialAverageIndicator(21);


IMethod hiloMethod = new HiLoMethod(9, hiloSLow);
IMethod simpleMethod = new SimpleMovingAverageMethod(exponencialSlower);
IMethod methodAverage = new MovingAverageMethod(exponencialFast, exponencialSlower);
IMethod macdMethod = new MACDMethod(exponencialFast, exponencialSlower, exponencialMoreFaster);

//var wallets = await CalculateMethod.Calculate(simpleMethod, symbols, initialDate, finalDate);

var wallets = await CalculateMethod.CalculateTwoMethods(macdMethod,simpleMethod, symbols, initialDate, finalDate, true, true);


foreach (WalletResult wallet in wallets)
{
    Console.WriteLine($"Symbol {wallet.Symbol}");
    Console.WriteLine($"Total Gain: {wallet.TotalGain}");
    Console.WriteLine($"Vallue Applicated: {wallet.ValueApplicated}");
    Console.WriteLine($"Win: {wallet.Win}");
    Console.WriteLine($"Loss: {wallet.Losses}");
    Console.WriteLine();
    Console.WriteLine();
}

decimal totalGain = wallets.Sum(w => w.TotalGain);
decimal totalWin = wallets.Sum(w => w.Win);
decimal totalLoss = wallets.Sum(w => w.Losses);

Console.WriteLine($"Total Gain: {totalGain}");
Console.WriteLine($"Total Win: {totalWin}");
Console.WriteLine($"Total Loss: {totalLoss}");









Console.ReadLine();
