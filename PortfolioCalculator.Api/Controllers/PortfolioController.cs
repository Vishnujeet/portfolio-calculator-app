using Microsoft.AspNetCore.Mvc;
using PortfolioCalculator.Core;
using PortfolioCalculator.Repository;

namespace PortfolioCalculator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly PortfolioService _portfolioService;
        private readonly IPortfolioRepository _repository;

        public PortfolioController(PortfolioService portfolioService, IPortfolioRepository repository)
        {
            _portfolioService = portfolioService;
            _repository = repository;
        }

        //[HttpGet("{investorId}/{date}")]
        //public async Task<IActionResult> GetPortfolioValue(string investorId, DateTime date)
        //{
        //    try
        //    {
        //        var value = await _portfolioService.CalculatePortfolioValueAsync(investorId, date);
        //        return Ok(new { InvestorId = investorId, Date = date.ToString("yyyy-MM-dd"), PortfolioValue = value });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = "Error retrieving portfolio value", Error = ex.Message });
        //    }
        //}

    }
}
