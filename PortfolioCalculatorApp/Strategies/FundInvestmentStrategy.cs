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
    public class FundInvestmentStrategy : IInvestmentStrategy
    {
        private readonly IPortfolioRepository _repository;
        private PortfolioService _portfolioService; 

        public FundInvestmentStrategy(IPortfolioRepository repository)
        {
            _repository = repository;
        }

        public void SetPortfolioService(PortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
        {
            try
            {
                if (_portfolioService == null)
                    throw new InvalidOperationException("PortfolioService has not been set.");

                var fundPercentage = await _repository.GetFundOwnershipAsync(investment.InvestorId, investment.InvestmentId, date);
                var fundValue = await _portfolioService.CalculatePortfolioValueAsync(investment.FondsInvestor, date);
                return fundPercentage * fundValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating fund investment value: {ex.Message}");
                return 0m;
            }
        }
    }

}
