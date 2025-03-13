# 📊 Investment Portfolio Calculator

## 🚀 Overview

The **Investment Portfolio Calculator** is a **console application** designed to calculate an investor's total portfolio value based on different investment types. It utilizes the **Strategy Pattern** to dynamically apply the correct calculation method for each type of investment, such as **Stocks, Real Estate, and Funds**.

## 🔥 Features

- 🏆 **Strategy Pattern Implementation** – Dynamically handles different investment types.
- 📡 **Repository Pattern for Data Retrieval** – Simulated data store for investment details.
- ⚡ **Asynchronous Processing** – Uses `async/await` for efficient data retrieval.
- 💰 **Supports Multiple Investment Types**:
  - **Stocks** (Shares in companies)
  - **Real Estate** (Land & Buildings)
  - **Funds** (Ownership percentage in funds)

## 📁 Project Structure

```
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
```

## 🛠 Installation

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/Vishnujeet/portfolio-calculator-app.git
cd investment-fund-calculator
```

## 🏗 Components

### 1️⃣ Portfolio Service
Manages portfolio calculations by retrieving the strategy for an investment type and determining the total portfolio value.
```csharp
public class PortfolioService
{
    public async Task<decimal> CalculateValueAsync(InvestmentData investment, DateTime date)
    {
        // Logic to calculate the fund value based on ownership percentage
    }
}
```

### 2️⃣ Investment Repository
Handles the retrieval of fund ownership and investment data.
```csharp
public interface IInvestmentRepository
{
    Task<decimal> GetFundOwnershipAsync(int investorId, int investmentId, DateTime date);
    Task<List<FundInvestment>> GetInvestmentsInFundAsync(int investmentId);
}
```

### 3️⃣ Investment Calculation Service
Calculates an investor's total portfolio value by applying investment-specific strategies.
```csharp
public class InvestmentCalculationService
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
        var fundPercentage = await _repository.GetFundOwnershipAsync(investment.InvestorId, investment.InvestmentId, date);
        if (fundPercentage <= 0)
        {
            _logger.LogDebug($"Invalid fund ownership for Investment {investment.InvestmentId}: {fundPercentage}%. Skipping calculation.");
            return 0.00m;
        }

        var fundInvestments = await _repository.GetInvestmentsInFundAsync(investment.InvestmentId);
        if (fundInvestments == null || !fundInvestments.Any())
        {
            _logger.LogDebug($"No investments found in fund {investment.InvestmentId}. Skipping calculation.");
            return 0.00m;
        }

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

        return fundPercentage * fundTotalValue;
    }
}
```

### 4️⃣ Logging
Uses `ILogger` for debugging and monitoring. Logs include:
- **Invalid ownership percentages**
- **Missing investments in funds**
- **Calculation errors**

#### Configure Logging in `Startup.cs`
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    });
}
```

## 🔄 How It Works

1️⃣ **User Inputs** a date (`yyyy-MM-dd`) and an Investor ID.
2️⃣ The system **fetches** related investments from the `PortfolioRepository`.
3️⃣ The **InvestmentStrategyContext** selects the correct strategy based on the investment type.
4️⃣ The corresponding **strategy** calculates the investment’s value.
5️⃣ The **PortfolioService** aggregates values and **displays** the total portfolio value.

## 🖥 Usage

1️⃣ **Run the Application**
2️⃣ Enter input in format: `YYYY-MM-DD;InvestorID`
3️⃣ View the calculated portfolio value.

**Example:**
```
Enter date (yyyy-MM-dd) and Investor ID separated by ';'
2024-03-11;INV123
Investor: INV123, Date: 2024-03-11, Portfolio Value: $150,000.00
```

## 🏗 Technologies Used

- **C# (.NET Core/Framework)**
- **Asynchronous Programming (`async/await`)**
- **SOLID Principles (Strategy & Repository Patterns)**

## 🔮 Future Enhancements

- ✅ **Database Integration** (Replace in-memory lists with a database)
- ✅ **Unit Tests for Strategies & Services**
- ✅ **Improved Error Handling & Logging**

## 📜 License

This project is open-source under the **MIT License**.

---

🚀 **Happy Investing!**
