using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Core
{
    public class PortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly InvestmentStrategyContext _strategyContext;

        public PortfolioService(IPortfolioRepository repository, InvestmentStrategyContext strategyContext)
        {
            _portfolioRepository = repository;
            _strategyContext = strategyContext;
        }

        public async Task<decimal> CalculatePortfolioValueAsync(string investorId, DateTime date)
        {
            try
            {
                var investments = await _portfolioRepository.GetInvestmentsByInvestorAsync(investorId);
                decimal totalValue = 0;

                foreach (var investment in investments)
                {
                    try
                    {
                        var strategy = _strategyContext.GetStrategy(investment.InvestmentType);
                        totalValue += await strategy.CalculateValueAsync(investment, date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing investment {investment.InvestmentId}: {ex.Message}");
                    }
                }

                 return totalValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Portfolio calculation failed for investor {investorId}: {ex.Message}");
                return 0m;
            }
        }
    }
}
