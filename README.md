# Investment Portfolio Calculator

## 📌 Overview

This is a **console application** that calculates the total value of an investor's portfolio based on different investment types. It implements the **Strategy Pattern** to dynamically apply the correct calculation method for each type of investment (Stocks, Real Estate, and Funds).

## 🚀 Features

- 📊 **Strategy Pattern Implementation**: Supports different investment types dynamically.
- 🔍 **Data Fetching with Repository Pattern**: Retrieves investment details from a simulated data store.
- ⚡ **Asynchronous Processing**: Uses async/await for efficient data retrieval.
- 🏦 **Supports Multiple Investment Types**:
  - Shares (Stock investments)
  - Real Estate (Land & Buildings)
  - Funds (Fund ownership percentage)

## Project Structure
```plaintext
- src/
  - InvestmentFundCalculator/
    - Services/
      - PortfolioService.cs
      - InvestmentCalculationService.cs
    - Repositories/
      - IInvestmentRepository.cs
      - InvestmentRepository.cs
    - Models/
      - InvestmentData.cs
    - Program.cs
    - Startup.cs
  - Tests/
    - InvestmentFundCalculator.Tests/
      - PortfolioServiceTests.cs
      - InvestmentCalculationServiceTests.cs
    - MockData/
      - MockInvestmentRepository.cs

## Installation

### Clone the repository:
Clone this repository to your local machine by running the following command:
```bash
git clone https://github.com/yourusername/investment-fund-calculator.git
cd investment-fund-calculator

##Components
1. PortfolioService
This service is responsible for managing and calculating the portfolio of an investor, including retrieving the strategy for a particular investment type and calculating the value of each investment in the fund.
``
public class PortfolioService
{
    public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
    {
        // Logic to calculate the fund value based on ownership percentage
    }
}
``
2. InvestmentRepository
This repository handles the retrieval of fund ownership and investment data. The methods include:

GetFundOwnershipAsync: Retrieves the ownership percentage of a specific investor in a given fund.
GetInvestmentsInFundAsync: Retrieves all investments within a specific fund.
``public interface IInvestmentRepository
{
    Task<decimal> GetFundOwnershipAsync(int investorId, int investmentId, DateTime date);
    Task<List<FundInvestment>> GetInvestmentsInFundAsync(int investmentId);
}
``
3. InvestmentCalculationService
This service calculates the total value of an investor's portfolio by looping through the investments within the fund and applying the strategies for each investment type.
``public class InvestmentCalculationService
{
    private readonly IInvestmentRepository _repository;
    private readonly PortfolioService _portfolioService;
    private readonly ILogger<InvestmentCalculationService> _logger;

    public InvestmentCalculationService(
        IInvestmentRepository repository,
        PortfolioService portfolioService,
        ILogger<InvestmentCalculationService> logger)
    {
        _repository = repository;
        _portfolioService = portfolioService;
        _logger = logger;
    }

    public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
    {
        // Retrieves ownership percentage and checks if it's valid.
        var fundPercentage = await _repository.GetFundOwnershipAsync(investment.InvestorId, investment.InvestmentId, date);
        if (fundPercentage <= 0)
        {
            _logger.LogDebug($"Invalid fund ownership for Investment {investment.InvestmentId}: {fundPercentage}%. Skipping calculation.");
            return 0.00m;
        }

        // Retrieve investments within the fund
        var fundInvestments = await _repository.GetInvestmentsInFundAsync(investment.InvestmentId);
        if (fundInvestments == null || !fundInvestments.Any())
        {
            _logger.LogDebug($"No investments found in fund {investment.InvestmentId}. Skipping calculation.");
            return 0.00m;
        }

        // Calculate fund value based on individual investments
        decimal fundTotalValue = 0;
        foreach (var fundInvestment in fundInvestments)
        {
            var strategy = _portfolioService.GetStrategy(fundInvestment.InvestmentType);
            try
            {
                fundTotalValue += await strategy.CalculateValueAsync(fundInvestment, date);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating value for investment {fundInvestment.InvestmentId}: {ex.Message}");
            }
        }

        // Multiply by ownership percentage to get the final value
        return fundPercentage * fundTotalValue;
    }
}
``
Logging
The project uses ILogger for logging purposes to assist in debugging and monitoring. Logs are outputted for various scenarios including:

Invalid ownership percentages.
No investments found in a fund.
Calculation errors.
To configure logging, add the following to your Startup.cs:
``public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    });
}
``

## 🛠 How It Works

1. The user **inputs** a date (yyyy-MM-dd) and an investor ID.
2. The program **fetches** the investments related to the investor from the `PortfolioRepository`.
3. The **InvestmentStrategyContext** selects the correct strategy based on investment type.
4. The corresponding **strategy** calculates the value of the investment.
5. The **PortfolioService** aggregates the values and **displays** the total portfolio value.

## 🖥 Usage

1. **Run the application**
2. Enter input in the format: `YYYY-MM-DD;InvestorID`
3. The portfolio value will be displayed.

Example:

```
Enter date (yyyy-MM-dd) and Investor ID separated by ';'
2024-03-11;INV123
Investor: INV123, Date: 2024-03-11, Portfolio Value: $150,000.00
```

## 🏗 Technologies Used

- **C# (.NET Core/Framework)**
- **Asynchronous Programming (async/await)**
- **SOLID Principles (Strategy & Repository Patterns)**

## 📌 Future Enhancements

- ✅ **Database Integration** (instead of in-memory lists)
- ✅ **Unit Tests for Strategies & Services**
- ✅ **Error Handling & Logging**

## 📜 License

This project is open-source under the **MIT License**.

---

🚀 **Happy Investing!**

