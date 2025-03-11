using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Core.Interface;
using PortfolioCalculator.Core.Strategies;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Core
{
    public class InvestmentStrategyContext
    {
        private readonly IPortfolioRepository _repository;
        private readonly Dictionary<string, IInvestmentStrategy> _strategies;
        private readonly FundInvestmentStrategy _fundStrategy;

        public InvestmentStrategyContext(IPortfolioRepository repository)
        {
            _repository = repository;
            _fundStrategy = new FundInvestmentStrategy(_repository); 

            _strategies = new Dictionary<string, IInvestmentStrategy>
            {
                { "Stock", new ShareInvestmentStrategy(_repository) },
                { "RealEstate", new RealEstateInvestmentStrategy(_repository) },
                { "Fonds", _fundStrategy }
            };
        }

        public void SetPortfolioService(PortfolioService portfolioService)
        {
            _fundStrategy.SetPortfolioService(portfolioService); 
        }

        public IInvestmentStrategy GetStrategy(string investmentType)
        {
            return _strategies.TryGetValue(investmentType, out var strategy)
                ? strategy
                : throw new NotImplementedException($"No strategy found for {investmentType}");
        }
    }

}
