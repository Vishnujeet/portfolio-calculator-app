using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortfolioCalculator.Core;
using PortfolioCalculator.Core.Strategies;
using PortfolioCalculator.Repository;
using PortfolioCalculator.Utils;

class Program
{
    static async Task Main(string[] args)
    {
        PortfolioService portfolioService;
        ILogger<Program> logger;
        InitializeServices(out portfolioService, out logger);

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

                // Get section-wise breakdown
                var totalPortfolioValue = await portfolioService.GetTotalPortfolioValueAsync(investorId, date);
                Console.WriteLine();
               Console.WriteLine("==========================================****====================================");
                Console.WriteLine($"Investor: {investorId}, Date: {date:yyyy-MM-dd}, Portfolio Value: {totalPortfolioValue:C}");

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

    private static void InitializeServices(out PortfolioService portfolioService, out ILogger<Program> logger)
    {
        // Set up Dependency Injection (DI) container
        var serviceCollection = new ServiceCollection();

        // Register logging
        serviceCollection.AddLogging(config => config.AddConsole());

        // Register CsvDataLoaderWrapper as the implementation of ICsvDataLoader
        serviceCollection.AddSingleton<ICsvDataLoader, CsvDataLoaderWrapper>();

        // Register PortfolioRepository synchronously
        serviceCollection.AddSingleton<IPortfolioRepository>(provider =>
        {
            var csvDataLoader = provider.GetRequiredService<ICsvDataLoader>();
            return PortfolioRepository.CreateAsync(csvDataLoader).GetAwaiter().GetResult();
        });

        // Register investment strategies
        serviceCollection.AddSingleton<ShareInvestmentStrategy>();
        serviceCollection.AddSingleton<RealEstateInvestmentStrategy>();

        // Register FundInvestmentStrategy with lazy initialization of PortfolioService
        serviceCollection.AddSingleton<FundInvestmentStrategy>(sp => new FundInvestmentStrategy(
            sp.GetRequiredService<IPortfolioRepository>(),
            new Lazy<PortfolioService>(() => sp.GetRequiredService<PortfolioService>())
        ));

        // Register InvestmentStrategyContext
        serviceCollection.AddSingleton<InvestmentStrategyContext>();

        // Register PortfolioService
        serviceCollection.AddSingleton<PortfolioService>();

        // Build the service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Resolve dependencies
        portfolioService = serviceProvider.GetRequiredService<PortfolioService>();
        logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var csvDataLoader = serviceProvider.GetRequiredService<ICsvDataLoader>();
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
