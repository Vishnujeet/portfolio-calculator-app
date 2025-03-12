using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Repository
{
    public interface IPortfolioRepository
    {
        Task<List<InvestmentData>> GetInvestmentsByInvestorAsync(string investorId);
        Task<decimal> GetStockPriceAsync(string isin, DateTime date);
        Task<decimal> GetSharesOwnedAsync(string investmentId, DateTime date);
        Task<decimal> GetPropertyValueAsync(string investmentId, string propertyType, DateTime date);
        Task<decimal> GetFundOwnershipAsync(string investorId, string investmentId, DateTime date);
        Task<decimal> GetFundMarketValueAsync(string fundId, DateTime date);
        Task<List<InvestmentData>> GetInvestmentsInFundAsync(string fundInvestmentId);

    }
}
