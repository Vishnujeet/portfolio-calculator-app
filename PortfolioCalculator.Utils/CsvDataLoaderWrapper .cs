using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Utils
{
    public class CsvDataLoaderWrapper : ICsvDataLoader
    {
        public Task<List<Transaction>> LoadTransactionsAsync(string filePath)
        {
            return CsvDataLoader.LoadTransactionsAsync(filePath);
        }

        public Task<List<InvestmentData>> LoadInvestmentsAsync(string filePath)
        {
            return CsvDataLoader.LoadInvestmentsAsync(filePath);
        }

        public Task<Dictionary<string, List<StockQuote>>> LoadStockQuotesAsync(string filePath)
        {
            return CsvDataLoader.LoadStockQuotesAsync(filePath);
        }
    }
}
