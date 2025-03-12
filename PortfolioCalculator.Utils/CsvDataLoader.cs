using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PortfolioCalculator.Domain;

namespace PortfolioCalculator.Utils
{
    public static class CsvDataLoader
    {
        public static async Task<List<Transaction>> LoadTransactionsAsync(string filePath)
        {
            return await Task.Run(() => LoadTransactions(filePath));
        }

        public static async Task<List<InvestmentData>> LoadInvestmentsAsync(string filePath)
        {
            return await Task.Run(() => LoadInvestments(filePath));
        }

        public static async Task<Dictionary<string, List<StockQuote>>> LoadStockQuotesAsync(string filePath)
        {
            return await Task.Run(() => LoadStockQuotes(filePath));
        }

        private static List<Transaction> LoadTransactions(string filePath)
        {
            var transactions = new List<Transaction>();
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Transaction file not found: {filePath}");
                return transactions;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1)) 
                {
                    var columns = line.Split(';');
                    if (columns.Length < 4) continue; 

                    transactions.Add(new Transaction
                    {
                        InvestmentId = columns[0],
                        Type = Enum.TryParse(columns[1], true, out InvestmentType investmentType)
                                ? investmentType
                                : throw new Exception($"Invalid investment type: {columns[1]}"),
                        Date = DateTime.ParseExact(columns[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Value = decimal.Parse(columns[3])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transactions: {ex.Message}");
            }

            return transactions;
        }

        private static List<InvestmentData> LoadInvestments(string filePath)
        {
            var investments = new List<InvestmentData>();
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Investment file not found: {filePath}");
                return investments;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var columns = line.Split(';');
                    if (columns.Length < 6) // Ensure all expected columns exist
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    if (!Enum.TryParse(columns[2], true, out InvestmentType investmentType))
                    {
                        Console.WriteLine($"Invalid investment type: {columns[2]}, skipping...");
                        continue;
                    }

                    var investment = new InvestmentData
                    {
                        InvestorId = columns[0],
                        InvestmentId = columns[1],
                        InvestmentType = investmentType,
                        ISIN = !string.IsNullOrWhiteSpace(columns[3]) ? columns[3] : null,
                        City = !string.IsNullOrWhiteSpace(columns[4]) ? columns[4] : null,
                        FondsInvestor = !string.IsNullOrWhiteSpace(columns[5]) ? columns[5] : null
                    };

                    investments.Add(investment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading investments: {ex.Message}");
            }

            return investments;
        }

        private static Dictionary<string, List<StockQuote>> LoadStockQuotes(string filePath)
        {
            var stockQuotes = new Dictionary<string, List<StockQuote>>();
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Stock quotes file not found: {filePath}");
                return stockQuotes;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    var columns = line.Split(';');
                    if (columns.Length < 3) continue;

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stock quotes: {ex.Message}");
            }

            return stockQuotes;
        }
    }
}
