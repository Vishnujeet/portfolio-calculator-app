using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioCalculator.Core;
using PortfolioCalculator.Core.Interface;
using PortfolioCalculator.Domain;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Tests
{
    public class PortfolioServiceTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private readonly Mock<InvestmentStrategyContext> _strategyContextMock;
        private readonly Mock<ILogger<PortfolioService>> _loggerMock;
        private readonly PortfolioService _portfolioService;

        public PortfolioServiceTests()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _strategyContextMock = new Mock<InvestmentStrategyContext>();
            _loggerMock = new Mock<ILogger<PortfolioService>>();
            _portfolioService = new PortfolioService(_portfolioRepositoryMock.Object, _strategyContextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CalculatePortfolioValueAsync_ShouldHandleErrorsGracefully()
        {
            // Arrange
            var investorId = "Investor1";
            var date = new DateTime(2023, 10, 1);

            _portfolioRepositoryMock
                .Setup(x => x.GetInvestmentsByInvestorAsync(investorId))
                .ReturnsAsync(new List<InvestmentData>
                {
                    new InvestmentData { InvestmentId = "1", InvestorId = investorId, InvestmentType = InvestmentType.Stock },
                    new InvestmentData { InvestmentId = "2", InvestorId = investorId, InvestmentType = InvestmentType.RealEstate }
                });

            var stockStrategyMock = new Mock<IInvestmentStrategy>();
            stockStrategyMock
                .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
                .ThrowsAsync(new Exception("Stock calculation failed"));

            var realEstateStrategyMock = new Mock<IInvestmentStrategy>();
            realEstateStrategyMock
                .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
                .ReturnsAsync(200m);

            _strategyContextMock
                .Setup(x => x.GetStrategy(InvestmentType.Stock))
                .Returns(stockStrategyMock.Object);

            _strategyContextMock
                .Setup(x => x.GetStrategy(InvestmentType.RealEstate))
                .Returns(realEstateStrategyMock.Object);

            // Act
            var result = await _portfolioService.CalculatePortfolioValueAsync(investorId, date);

            // Assert
            result.Should().Be(200m); // Only Real Estate value is added
            _loggerMock.Verify(
                x => x.LogError(It.IsAny<Exception>(), "Error processing investment {InvestmentId}", "1"),
                Times.Once);
        }

        //[Fact]
        //public async Task CalculateRealEstateValueAsync_ShouldReturnCorrectRealEstateValue()
        //{
        //    // Arrange
        //    var investorId = "Investor1";
        //    var date = new DateTime(2023, 10, 1);

        //    _portfolioRepositoryMock
        //        .Setup(x => x.GetInvestmentsByInvestorAsync(investorId))
        //        .ReturnsAsync(new List<InvestmentData>
        //        {
        //            new InvestmentData { InvestmentId = "1", InvestorId = investorId, InvestmentType = InvestmentType.RealEstate },
        //            new InvestmentData { InvestmentId = "2", InvestorId = investorId, InvestmentType = InvestmentType.RealEstate }
        //        });

        //    var realEstateStrategyMock = new Mock<IInvestmentStrategy>();
        //    realEstateStrategyMock
        //        .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
        //        .ReturnsAsync(150m);

        //    _strategyContextMock
        //        .Setup(x => x.GetStrategy(InvestmentType.RealEstate))
        //        .Returns(realEstateStrategyMock.Object);

        //    // Act
        //    var result = await _portfolioService.CalculateRealEstateValueAsync(investorId, date);

        //    // Assert
        //    result.Should().Be(300m); // 150 (Real Estate 1) + 150 (Real Estate 2)
        //}

        //[Fact]
        //public async Task CalculateFondsValueAsync_ShouldReturnCorrectFondsValue()
        //{
        //    // Arrange
        //    var investorId = "Investor1";
        //    var date = new DateTime(2023, 10, 1);

        //    _portfolioRepositoryMock
        //        .Setup(x => x.GetInvestmentsByInvestorAsync(investorId))
        //        .ReturnsAsync(new List<InvestmentData>
        //        {
        //            new InvestmentData { InvestmentId = "1", InvestorId = investorId, InvestmentType = InvestmentType.Fonds },
        //            new InvestmentData { InvestmentId = "2", InvestorId = investorId, InvestmentType = InvestmentType.Fonds }
        //        });

        //    var fondsStrategyMock = new Mock<IInvestmentStrategy>();
        //    fondsStrategyMock
        //        .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
        //        .ReturnsAsync(200m);

        //    _strategyContextMock
        //        .Setup(x => x.GetStrategy(InvestmentType.Fonds))
        //        .Returns(fondsStrategyMock.Object);

        //    // Act
        //    var result = await _portfolioService.CalculateFondsValueAsync(investorId, date);

        //    // Assert
        //    result.Should().Be(400m); // 200 (Fonds 1) + 200 (Fonds 2)
        //}

        [Fact]
        public async Task GetTotalPortfolioValueAsync_ShouldReturnSumOfAllInvestments()
        {
            // Arrange
            var investorId = "Investor1";
            var date = new DateTime(2023, 10, 1);

            _portfolioRepositoryMock
                .Setup(x => x.GetInvestmentsByInvestorAsync(investorId))
                .ReturnsAsync(new List<InvestmentData>
                {
            new InvestmentData { InvestmentId = "1", InvestorId = investorId, InvestmentType = InvestmentType.Stock },
            new InvestmentData { InvestmentId = "2", InvestorId = investorId, InvestmentType = InvestmentType.RealEstate },
            new InvestmentData { InvestmentId = "3", InvestorId = investorId, InvestmentType = InvestmentType.Fonds }
                });

            var stockStrategyMock = new Mock<IInvestmentStrategy>();
            stockStrategyMock
                .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
                .ReturnsAsync(100m);

            var realEstateStrategyMock = new Mock<IInvestmentStrategy>();
            realEstateStrategyMock
                .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
                .ReturnsAsync(200m);

            var fondsStrategyMock = new Mock<IInvestmentStrategy>();
            fondsStrategyMock
                .Setup(x => x.CalculateValueAsync(It.IsAny<InvestmentData>(), date))
                .ReturnsAsync(300m);

            _strategyContextMock
                .Setup(x => x.GetStrategy(InvestmentType.Stock))
                .Returns(stockStrategyMock.Object);

            _strategyContextMock
                .Setup(x => x.GetStrategy(InvestmentType.RealEstate))
                .Returns(realEstateStrategyMock.Object);

            _strategyContextMock
                .Setup(x => x.GetStrategy(InvestmentType.Fonds))
                .Returns(fondsStrategyMock.Object);

            // Act
            var result = await _portfolioService.GetTotalPortfolioValueAsync(investorId, date);

            // Assert
            result.Should().Be(600m); // 100 (Stock) + 200 (Real Estate) + 300 (Fonds)
        }

    }
}