using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Repository
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<InvestmentData>> GetInvestmentsByInvestorAsync(string investorId);
        Task<decimal> GetStockPriceAsync(string stockId, DateTime date);
        Task<int> GetSharesOwnedAsync(string investmentId, DateTime date);
        Task<decimal> GetPropertyValueAsync(string investmentId, string propertyType, DateTime date);
        Task<decimal> GetFundOwnershipAsync(string investorId, string fundId, DateTime date);
    }
}
