using PortfolioCalculator.Core.Interface;
using PortfolioCalculator.Core.Strategies;
using PortfolioCalculator.Domain;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Core
{
    public class InvestmentStrategyContext
    {
        private readonly Dictionary<InvestmentType, IInvestmentStrategy> _strategies;

        public InvestmentStrategyContext(
            ShareInvestmentStrategy shareStrategy,
            RealEstateInvestmentStrategy realEstateStrategy,
            FundInvestmentStrategy fundStrategy)
        {
            _strategies = new Dictionary<InvestmentType, IInvestmentStrategy>
        {
            { InvestmentType.Stock, shareStrategy },
            { InvestmentType.RealEstate, realEstateStrategy },
            { InvestmentType.Fonds, fundStrategy }
        };
        }

        public IInvestmentStrategy GetStrategy(InvestmentType investmentType)
        {
            return _strategies.TryGetValue(investmentType, out var strategy)
                ? strategy
                : throw new NotImplementedException($"No strategy found for {investmentType}");
        }
    }

}
