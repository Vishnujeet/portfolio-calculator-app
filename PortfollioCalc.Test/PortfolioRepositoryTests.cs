using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PortfolioCalculator.Domain;
using PortfolioCalculator.Repository;
using PortfolioCalculator.Utils;
using Xunit;

namespace PortfolioCalculator.Tests
{
    public class PortfolioRepositoryTests
    {
        private readonly Mock<ICsvDataLoader> _csvDataLoaderMock;

        public PortfolioRepositoryTests()
        {
            _csvDataLoaderMock = new Mock<ICsvDataLoader>();
        }

        [Fact]
        public async Task CreateAsync_ShouldLoadDataSuccessfully()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { InvestmentId = "1", Date = DateTime.Now, Type = InvestmentType.Shares, Value = 100 }
            };
            var investments = new List<InvestmentData>
            {
                new InvestmentData { InvestmentId = "1", InvestorId = "Investor1" }
            };
            var stockQuotes = new Dictionary<string, List<StockQuote>>
            {
                { "ISIN1", new List<StockQuote> { new StockQuote { Date = DateTime.Now, PricePerShare = 50 } } }
            };

            _csvDataLoaderMock.Setup(x => x.LoadTransactionsAsync(It.IsAny<string>())).ReturnsAsync(transactions);
            _csvDataLoaderMock.Setup(x => x.LoadInvestmentsAsync(It.IsAny<string>())).ReturnsAsync(investments);
            _csvDataLoaderMock.Setup(x => x.LoadStockQuotesAsync(It.IsAny<string>())).ReturnsAsync(stockQuotes);

            // Act
            var repository = await PortfolioRepository.CreateAsync(_csvDataLoaderMock.Object);

            // Assert
            repository.Should().NotBeNull();
            var investmentsByInvestor = await repository.GetInvestmentsByInvestorAsync("Investor1");
            investmentsByInvestor.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetInvestmentsByInvestorAsync_ShouldReturnInvestmentsForInvestor()
        {
            // Arrange
            var investments = new List<InvestmentData>
            {
                new InvestmentData { InvestmentId = "1", InvestorId = "Investor1" },
                new InvestmentData { InvestmentId = "2", InvestorId = "Investor2" }
            };
            _csvDataLoaderMock.Setup(x => x.LoadInvestmentsAsync(It.IsAny<string>())).ReturnsAsync(investments);

            var repository = await PortfolioRepository.CreateAsync(_csvDataLoaderMock.Object);

            // Act
            var result = await repository.GetInvestmentsByInvestorAsync("Investor1");

            // Assert
            result.Should().HaveCount(1);
            result.First().InvestmentId.Should().Be("1");
        }

        [Fact]
        public async Task GetStockPriceAsync_ShouldReturnStockPriceForGivenDate()
        {
            // Arrange
            var stockQuotes = new Dictionary<string, List<StockQuote>>
            {
                { "ISIN1", new List<StockQuote> { new StockQuote { Date = new DateTime(2023, 10, 1), PricePerShare = 50 } } }
            };
            _csvDataLoaderMock.Setup(x => x.LoadStockQuotesAsync(It.IsAny<string>())).ReturnsAsync(stockQuotes);

            var repository = await PortfolioRepository.CreateAsync(_csvDataLoaderMock.Object);

            // Act
            var result = await repository.GetStockPriceAsync("ISIN1", new DateTime(2023, 10, 1));

            // Assert
            result.Should().Be(50);
        }

    }
}