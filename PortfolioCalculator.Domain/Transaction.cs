using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioCalculator.Domain
{
    public enum InvestmentType
    {
        Stock,
        RealEstate,
        Fonds,
        Percentage,
        Estate,
        Shares,
        Building,
        Sell
    }

    public class Transaction
    {
        public string InvestmentId { get; set; }
        public InvestmentType Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }

}
