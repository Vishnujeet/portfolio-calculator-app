using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Utils
{
    public interface ICsvDataLoader
    {
        Task<List<Transaction>> LoadTransactionsAsync(string filePath);
        Task<List<InvestmentData>> LoadInvestmentsAsync(string filePath);
        Task<Dictionary<string, List<StockQuote>>> LoadStockQuotesAsync(string filePath);
    }
}
