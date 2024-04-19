using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialHelper.Core.Entities
{
    public class GenericIntermediaryIndicator
    {
        public decimal MaxValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal Start { get; set; }
        public decimal Stop { get; set; }
        public decimal Average { get; set; }
        public DateTime Data { get; set; }
    }
}
