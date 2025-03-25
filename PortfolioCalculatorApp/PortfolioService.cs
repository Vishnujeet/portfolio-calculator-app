using Microsoft.Extensions.Logging;
using PortfolioCalculator.Domain;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Core
{
    public class PortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly InvestmentStrategyContext _strategyContext;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(IPortfolioRepository repository, InvestmentStrategyContext strategyContext, ILogger<PortfolioService> logger)
        {
            _portfolioRepository = repository;
            _strategyContext = strategyContext;
            _logger = logger;
        }
        public async Task<decimal> GetTotalPortfolioValueAsync(string investorId, DateTime date)
        {
            var breakdown = new Dictionary<string, decimal>
            {
                { "Stocks", await CalculateStockValueAsync(investorId, date) },
                { "Real Estate", await CalculateRealEstateValueAsync(investorId, date) },
                { "Fonds", await CalculateFondsValueAsync(investorId, date) }
            };

            return breakdown.Values.Sum();
        }

        // Method to Calculate Stock Investments
        private async Task<decimal> CalculateStockValueAsync(string investorId, DateTime date)
        {
            var investments = await _portfolioRepository.GetInvestmentsByInvestorAsync(investorId);
            decimal totalStockValue = 0m;

            foreach (var investment in investments.Where(i => i.InvestmentType == InvestmentType.Stock))
            {
                var strategy = _strategyContext.GetStrategy(InvestmentType.Stock);
                totalStockValue += await strategy.CalculateValueAsync(investment, date);
            }

            return totalStockValue;
        }

        // Method to Calculate Real Estate Investments
        private async Task<decimal> CalculateRealEstateValueAsync(string investorId, DateTime date)
        {
            var investments = await _portfolioRepository.GetInvestmentsByInvestorAsync(investorId);
            decimal totalRealEstateValue = 0m;

            foreach (var investment in investments.Where(i => i.InvestmentType == InvestmentType.RealEstate))
            {
                var strategy = _strategyContext.GetStrategy(InvestmentType.RealEstate);
                totalRealEstateValue += await strategy.CalculateValueAsync(investment, date);
            }

            return totalRealEstateValue;
        }

        // Method to Calculate Fonds (Mutual Funds) Investments
        private async Task<decimal> CalculateFondsValueAsync(string investorId, DateTime date)
        {
            var investments = await _portfolioRepository.GetInvestmentsByInvestorAsync(investorId);
            decimal totalFondsValue = 0m;

            foreach (var investment in investments.Where(i => i.InvestmentType == InvestmentType.Fonds))
            {
                var strategy = _strategyContext.GetStrategy(InvestmentType.Fonds);
                totalFondsValue += await strategy.CalculateValueAsync(investment, date);
            }

            return totalFondsValue;
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
                        _logger.LogError(ex, "Error processing investment {InvestmentId}", investment.InvestmentId);
                    }
                }

                return totalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Portfolio calculation failed for investor {InvestorId}", investorId);
                return 0m;
            }
        }
    }
}
