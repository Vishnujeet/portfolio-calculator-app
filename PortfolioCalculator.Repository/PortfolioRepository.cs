using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PortfolioCalculator.Domain;
using Transaction = PortfolioCalculator.Domain.Transaction;

namespace PortfolioCalculator.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private List<Transaction> _transactions;
        private List<InvestmentData> _investments;
        private Dictionary<string, List<StockQuote>> _stockQuotes;

        public PortfolioRepository() { }
        
        public static async Task<PortfolioRepository> CreateAsync()
        {
            var repository = new PortfolioRepository();
            repository._transactions = await repository.LoadTransactionsAsync();
            repository._investments = await repository.LoadInvestmentsAsync();
            repository._stockQuotes = await repository.LoadStockQuotesAsync();
            return repository;
        }

        public async Task<IEnumerable<InvestmentData>> GetInvestmentsByInvestorAsync(string investorId)
        {
            return await Task.Run(() => _investments?.Where(i => i.InvestorId == investorId) ?? Enumerable.Empty<InvestmentData>());
        }
        public Task<decimal> GetStockPriceAsync(string stockId, DateTime date)
        {
            var price = _stockQuotes?.TryGetValue(stockId, out var quotes) == true
                ? quotes.Where(q => q.Date <= date).OrderByDescending(q => q.Date).FirstOrDefault()?.PricePerShare ?? 0m
                : 0m;
            return Task.FromResult(price);
        }

        public Task<int> GetSharesOwnedAsync(string investmentId, DateTime date)
        {
            var shares = _transactions?.Where(t => t.InvestmentId == investmentId && t.Type == "Stock" && t.Date <= date)
                                       .Sum(t => (int)t.Value) ?? 0;
            return Task.FromResult(shares);
        }

        public Task<decimal> GetPropertyValueAsync(string investmentId, string propertyType, DateTime date)
        {
            var value = _transactions?.Where(t => t.InvestmentId == investmentId && t.Type == propertyType && t.Date <= date)
                                      .OrderByDescending(t => t.Date)
                                      .Select(t => t.Value)
                                      .FirstOrDefault() ?? 0m;
            return Task.FromResult(value);
        }

        public Task<decimal> GetFundOwnershipAsync(string investorId, string fundId, DateTime date)
        {
            var ownership = _transactions?.Where(t => t.InvestmentId == fundId && t.Type == "Percentage" && t.Date <= date)
                                          .Sum(t => t.Value) ?? 0m;
            return Task.FromResult(ownership);
        }

        private async Task<List<Transaction>> LoadTransactionsAsync()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Transactions.csv");
            return await Task.Run(() => LoadTransactionsFromCsv(filePath));
        }

        private async Task<List<InvestmentData>> LoadInvestmentsAsync()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Investments.csv");
            return await Task.Run(() => LoadInvestmentsFromCsv(filePath));
        }

        private async Task<Dictionary<string, List<StockQuote>>> LoadStockQuotesAsync()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Quotes.csv");
            return await Task.Run(() => LoadStockQuotesFromCsv(filePath));
        }

        private List<Transaction> LoadTransactionsFromCsv(string filePath)
        {
            try
            {
                var transactions = new List<Transaction>();


                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines.Skip(1)) 
                    {
                        var columns = line.Split(';');
                        transactions.Add(new Transaction
                        {
                            InvestmentId = columns[0],
                            Type = columns[1],
                            Date = DateTime.ParseExact(columns[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            Value = decimal.Parse(columns[3])
                        });
                    }
                }
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transactions: {ex.Message}");
                return new List<Transaction>();
            }
        }
        private List<InvestmentData> LoadInvestmentsFromCsv(string filePath)
        {
            try
            {
                var investments = new List<InvestmentData>();

                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(';');
                        investments.Add(new InvestmentData
                        {
                            InvestorId = columns[0],
                            InvestmentId = columns[1],
                            InvestmentType = columns[2],
                            ISIN = columns.Length > 3 ? columns[3] : null,
                            FondsInvestor = columns.Length > 4 ? columns[4] : null
                        });
                    }
                }
                return investments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading investments: {ex.Message}");
                return new List<InvestmentData>();
            }
        }
        private Dictionary<string, List<StockQuote>> LoadStockQuotesFromCsv(string filePath)
        {
            try
            {
                var stockQuotes = new Dictionary<string, List<StockQuote>>();


                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(';');
                        var stockQuote = new StockQuote
                        {
                            ISIN = columns[0],
                            Date = DateTime.ParseExact(columns[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            PricePerShare = decimal.Parse(columns[2])
                        };

                        if (!stockQuotes.ContainsKey(stockQuote.ISIN))
                            stockQuotes[stockQuote.ISIN] = new List<StockQuote>();

                        stockQuotes[stockQuote.ISIN].Add(stockQuote);
                    }
                }
                return stockQuotes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stock quotes: {ex.Message}");
                return new Dictionary<string, List<StockQuote>>();
            }
        }
    }

}
