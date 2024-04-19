namespace FinancialHelper.FastTester
{
    public class WalletResult
    {
        public string Symbol { get; set; }
        public decimal ValueApplicated { get; set; }
        public decimal TotalGain { get; set; }

        public long Win { get; set; }
        public long Losses { get; set; }

        public decimal totalWin { get; set; }

    }
}