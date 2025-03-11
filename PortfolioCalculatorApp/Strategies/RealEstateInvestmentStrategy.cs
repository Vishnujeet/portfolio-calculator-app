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
    public class RealEstateInvestmentStrategy : IInvestmentStrategy
    {
        private readonly IPortfolioRepository _repository;

        public RealEstateInvestmentStrategy(IPortfolioRepository repository) => _repository = repository;

        public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
        {
            try
            {
                var landValue = await _repository.GetPropertyValueAsync(investment.InvestmentId, "Estate", date);
                var buildingValue = await _repository.GetPropertyValueAsync(investment.InvestmentId, "Building", date);
                return landValue + buildingValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating real estate investment value: {ex.Message}");
                return 0m;
            }
        }
    }
}
