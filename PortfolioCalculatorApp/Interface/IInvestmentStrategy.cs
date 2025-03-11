using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Core.Interface
{
    public interface IInvestmentStrategy
    {
        Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date);
    }

}
