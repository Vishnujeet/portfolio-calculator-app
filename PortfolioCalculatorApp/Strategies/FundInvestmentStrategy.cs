﻿using System;
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
        private readonly Lazy<PortfolioService> _portfolioService; 
        private readonly InvestmentStrategyContext _strategyContext; 

        public FundInvestmentStrategy(IPortfolioRepository repository, Lazy<PortfolioService> portfolioService)
        {
            _repository = repository;
            _portfolioService = portfolioService;
        }

        public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
        {
            try
            {
                // Fetch fund ownership percentage
                decimal fundPercentage = await _repository.GetFundOwnershipAsync(investment.InvestorId, investment.InvestmentId, date);

                if (fundPercentage <= 0)
                {
                    return 0m; // No need to calculate if ownership is 0 or negative
                }

                var fundInvestments = await _repository.GetInvestmentsInFundAsync(investment.InvestmentId);

                if (fundInvestments == null || !fundInvestments.Any())
                {
                    return 0m;
                }

                decimal fundTotalValue = 0;
                foreach (var fundInvestment in fundInvestments)
                {
                    var strategy = _strategyContext.GetStrategy(fundInvestment.InvestmentType);

                    decimal investmentValue = await strategy.CalculateValueAsync(fundInvestment, date);
                    fundTotalValue += investmentValue;  // Accumulate total fund value
                }

                decimal result = fundPercentage * fundTotalValue;

                return result;  // Return calculated value
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Fund calculation failed for {investment.InvestmentId}: {ex.Message}");
                return 0m;  
            }
        }



    }


}
