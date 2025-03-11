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

## 🏗 Project Structure

```
📌 Console Application (Program.cs)
   ├── User Input Handling
   ├── Calls PortfolioService
   ├── Outputs Portfolio Value
   
📌 Business Logic
   ├── PortfolioService.cs (Calculates total value)
   ├── InvestmentStrategyContext.cs (Chooses strategy)
   ├── Strategies:
       ├── ShareInvestmentStrategy.cs
       ├── RealEstateInvestmentStrategy.cs
       ├── FundInvestmentStrategy.cs

📌 Data Access Layer (Repository)
   ├── IPortfolioRepository.cs (Interface)
   ├── PortfolioRepository.cs (Fetches investment data)

📌 Data Models
   ├── InvestmentData.cs
   ├── Transaction.cs
   ├── StockQuote.cs
```

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

