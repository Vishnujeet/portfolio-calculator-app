//using PortfolioCalculator.Core.Interface;
//using PortfolioCalculator.Core;
//using PortfolioCalculator.Domain;
//using PortfolioCalculator.Repository;
//using Moq;

//namespace PortfollioCalc.Test
//{
//    public class PortfolioServiceTests
//    {
//        private readonly Mock<IPortfolioRepository> _mockRepository;
//        private readonly PortfolioService _portfolioService;
//        private readonly InvestmentStrategyContext _strategyContext;

//        public PortfolioServiceTests()
//        {
//            _mockRepository = new Mock<IPortfolioRepository>();

//            var strategies = new Dictionary<string, IInvestmentStrategy>
//        {
//            { "Shares", new Mock<IInvestmentStrategy>().Object },
//            { "Estate", new Mock<IInvestmentStrategy>().Object },
//            { "Fonds", new Mock<IInvestmentStrategy>().Object }
//        };

//            _strategyContext = new InvestmentStrategyContext(_mockRepository.Object);
//            _portfolioService = new PortfolioService(_mockRepository.Object, _strategyContext);
//        }

//        [Fact]
//        public async Task CalculatePortfolioValueAsync_ShouldReturnCorrectValue()
//        {
//            // Arrange
//            var investorId = "Investor1";
//            var date = DateTime.UtcNow;

//            var investments = new List<InvestmentData>
//        {
//            new InvestmentData { InvestorId = investorId, InvestmentId = "INV001" },
//            new InvestmentData { InvestorId = investorId, InvestmentId = "INV002" }
//        };

//            _mockRepository.Setup(repo => repo.GetInvestmentsByInvestorAsync(investorId))
//                           .ReturnsAsync(investments);

//            // Act
//            decimal result = await _portfolioService.CalculatePortfolioValueAsync(investorId, date);

//            // Assert
//            Assert.Equal(0m, result);
//        }
//    }
//}