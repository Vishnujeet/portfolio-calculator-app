using PortfolioCalculator.Domain;
using PortfolioCalculator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioCalculator.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private List<Transaction> _transactions;
        private List<InvestmentData> _investments;
        private Dictionary<string, List<StockQuote>> _stockQuotes;

        private PortfolioRepository() { }

        public static async Task<PortfolioRepository> CreateAsync()
        {
            var repository = new PortfolioRepository();

            try
            {
                repository._transactions = await CsvDataLoader.LoadTransactionsAsync("Data/Transactions.csv");
                repository._investments = await CsvDataLoader.LoadInvestmentsAsync("Data/Investments.csv");
                repository._stockQuotes = await CsvDataLoader.LoadStockQuotesAsync("Data/Quotes.csv");

                Console.WriteLine("Data successfully loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                repository._transactions = new List<Transaction>();
                repository._investments = new List<InvestmentData>();
                repository._stockQuotes = new Dictionary<string, List<StockQuote>>();
            }

            return repository;
        }

        public async Task<List<InvestmentData>> GetInvestmentsByInvestorAsync(string investorId)
        {
            try
            {
              var result= await Task.FromResult(_investments.Where(i => i.InvestorId == investorId).ToList());
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving investments for investor {investorId}: {ex.Message}");
                return new List<InvestmentData>();
            }
        }

        public async Task<decimal> GetStockPriceAsync(string isin, DateTime date)
        {
            try
            {
                if (_stockQuotes.TryGetValue(isin, out var quotes) && quotes.Any())
                {
                    var stock = quotes
                        .Where(q => q.Date.Date <= date.Date)
                        .OrderByDescending(q => q.Date) // Get the last available price
                        .FirstOrDefault();

                    return stock?.PricePerShare ?? 0m;
                }

                return 0m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error retrieving stock price for {isin} on {date:yyyy-MM-dd}: {ex.Message}");
                return 0m;
            }
        }

        public async Task<decimal> GetSharesOwnedAsync(string investmentId, DateTime date)
        {
            var transactions = await GetTransactionsAsync(investmentId, date);
            return transactions.Where(t => t.Type == InvestmentType.Shares).Sum(t => t.Value);
        }

        public async Task<decimal> GetPropertyValueAsync(string investmentId, string type, DateTime date)
        {
            var transactions = await GetTransactionsAsync(investmentId, date);
            return transactions.Where(t => t.Type.ToString() == type).Sum(t => t.Value);
        }
        public async Task<List<Transaction>> GetTransactionsAsync(string investmentId, DateTime date)
        {
            try
            {
                return _transactions
                    .Where(t => t.InvestmentId == investmentId && t.Date <= date)
                    .OrderBy(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving transactions for investment {investmentId}: {ex.Message}");
                return new List<Transaction>(); // Return empty list if there's an error
            }
        }


        public async Task<decimal> GetFundOwnershipAsync(string investorId, string investmentId, DateTime date)
        {
            try
            {
                var ownershipTransactions = _transactions
                    .Where(t => t.InvestmentId == investmentId && t.Type == InvestmentType.Percentage && t.Date <= date)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return ownershipTransactions.FirstOrDefault()?.Value ?? 0m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving fund ownership for {investorId}: {ex.Message}");
                return 0m;
            }
        }

        public async Task<List<InvestmentData>> GetInvestmentsInFundAsync(string fundInvestmentId)
        {
            try
            {
                return _investments
                    .Where(i => i.FondsInvestor == fundInvestmentId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving investments in fund {fundInvestmentId}: {ex.Message}");
                return new List<InvestmentData>(); // Return empty list if there's an error
            }
        }


        public async Task<decimal> GetFundMarketValueAsync(string fundId, DateTime date)
        {
            try
            {
                var relevantTransactions = _transactions
                    .Where(t => t.InvestmentId == fundId && t.Type == InvestmentType.Percentage && t.Date <= date)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                decimal totalOwnership = relevantTransactions.FirstOrDefault()?.Value ?? 0m;
                decimal totalMarketValue = totalOwnership * await GetStockPriceAsync(fundId, date);

                return totalMarketValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving fund market value for {fundId}: {ex.Message}");
                return 0m;
            }
        }
    }
}
