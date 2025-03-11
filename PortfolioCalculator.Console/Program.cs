using PortfolioCalculator.Core;
using PortfolioCalculator.Repository;

class Program
{
    static async Task Main(string[] args)
    {
        IPortfolioRepository repository = await PortfolioRepository.CreateAsync();
        var strategyContext = new InvestmentStrategyContext(repository);
        var portfolioService = new PortfolioService(repository, strategyContext);

        strategyContext.SetPortfolioService(portfolioService); 

        Console.WriteLine("Enter date (yyyy-MM-dd) and Investor ID separated by ';'");
        var line = Console.ReadLine();

        while (!string.IsNullOrWhiteSpace(line))
        {
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

                var cts = new CancellationTokenSource();
                Task loadingTask = ShowLoadingAnimation(cts.Token);

                decimal portfolioValue = await portfolioService.CalculatePortfolioValueAsync(investorId, date);

                cts.Cancel();
                await loadingTask;

                Console.WriteLine($"Investor: {investorId}, Date: {date:yyyy-MM-dd}, Portfolio Value: {portfolioValue:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            line = Console.ReadLine();
        }
    }
    static async Task ShowLoadingAnimation(CancellationToken cancellationToken)
    {
        string[] animation = { "|", "/", "-", "\\" };
        int counter = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write($"\rCalculating portfolio value {animation[counter++ % animation.Length]}");
            await Task.Delay(200); 
        }

        Console.Write("\r"); 
    }

}
