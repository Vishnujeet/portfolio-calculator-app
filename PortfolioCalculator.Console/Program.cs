using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortfolioCalculator.Core;
using PortfolioCalculator.Core.Strategies;
using PortfolioCalculator.Repository;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up Dependency Injection (DI) container
        var serviceProvider = new ServiceCollection()
     .AddLogging(config => config.AddConsole()) // Logging support
     .AddSingleton<IPortfolioRepository>(await PortfolioRepository.CreateAsync()) // Load repository
     .AddSingleton<ShareInvestmentStrategy>()
     .AddSingleton<RealEstateInvestmentStrategy>()
     .AddSingleton<FundInvestmentStrategy>(sp => new FundInvestmentStrategy(
         sp.GetRequiredService<IPortfolioRepository>(),
         new Lazy<PortfolioService>(() => sp.GetRequiredService<PortfolioService>())
     )) // Lazy inject PortfolioService
     .AddSingleton<InvestmentStrategyContext>()
     .AddSingleton<PortfolioService>() // Register PortfolioService separately
     .BuildServiceProvider();


        // Resolve dependencies
        var portfolioService = serviceProvider.GetRequiredService<PortfolioService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        Console.WriteLine("Enter date (yyyy-MM-dd) and Investor ID separated by ';'");
        var line = Console.ReadLine();

        while (!string.IsNullOrWhiteSpace(line))
        {
            //// Start a loading animation
            var cts = new CancellationTokenSource();
            Task loadingTask = ShowLoadingAnimation(cts.Token);
            try
            {
                var input = line.Split(';');
                if (input.Length != 2 || !DateTime.TryParse(input[0], out DateTime date))
                {
                    Console.WriteLine("Invalid input format. Please enter in yyyy-MM-dd;InvestorID format.");
                    line = Console.ReadLine();
                    continue;
                }

                var investorId = input[1];

               

                //decimal portfolioValue = await portfolioService.CalculatePortfolioValueAsync(investorId, date);


                //// Stop the loading animation
                //cts.Cancel();
                //await loadingTask;

                //Console.WriteLine($"Investor: {investorId}, Date: {date:yyyy-MM-dd}, Portfolio Value: {portfolioValue:C}");
                // Get section-wise breakdown
                var breakdown = await portfolioService.GetPortfolioBreakdownAsync(investorId, date);
                decimal totalPortfolioValue = breakdown.Values.Sum();
               


                Console.WriteLine("\n===== Portfolio Breakdown =====");
                foreach (var section in breakdown)
                {
                    Console.WriteLine($"{section.Key}: {section.Value:C}");
                }
                Console.WriteLine("===============================");
                Console.WriteLine($"Total Portfolio Value: {totalPortfolioValue:C}");
                Console.WriteLine($"Investor: {investorId}, Date: {date:yyyy-MM-dd}, Portfolio Value: {totalPortfolioValue:C}");

                //cts.Cancel();  // Stop animation
                //await loadingTask;  // Ensure animation task exits
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while calculating portfolio value.");
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                cts.Cancel();  // Stop the animation
                await loadingTask;  // Ensure animation finishes gracefully
            }
            line = Console.ReadLine();
        }
    }

    private static async Task ShowLoadingAnimation(CancellationToken token)
    {
        string[] animation = { "/", "-", "\\", "|" };
        int counter = 0;

        try
        {
            while (!token.IsCancellationRequested)
            {
                Console.Write($"\rCalculating portfolio value... {animation[counter % animation.Length]}");
                counter++;
                await Task.Delay(500, token); // Can throw TaskCanceledException
            }
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            Console.Write("\rCalculation completed!                \n");
        }
    }

}
