using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Core.Interface;
using PortfolioCalculator.Domain;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Core.Strategies
{
    public class ShareInvestmentStrategy : IInvestmentStrategy
    {
        private readonly IPortfolioRepository _repository;

        public ShareInvestmentStrategy(IPortfolioRepository repository) => _repository = repository;

        public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
        {
            try
            {
                var stockPrice = await _repository.GetStockPriceAsync(investment.ISIN, date);
                var sharesOwned = await _repository.GetSharesOwnedAsync(investment.InvestmentId, date);
                return sharesOwned * stockPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating share investment value: {ex.Message}");
                return 0m;
            }
        }
    }

}
