using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamiNation.API.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly Seeder _testSeeder;
        private readonly AppDbContext _context;

        public SeedController(Seeder testSeeder, AppDbContext context)
        {
            _testSeeder = testSeeder;
            _context = context;
        }

        [HttpPost("seed-spanish")]
        public async Task<IActionResult> SeedTestAsync()
        {
            //Docker path
            var seedFilePath =  _testSeeder.ResolveSeedPath("test-otis.json");
            var seedFileScoreRangePath = _testSeeder.ResolveSeedPath("classifications.json");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _testSeeder.SeedTestFromJsonAsync(seedFilePath, seedFileScoreRangePath);
                    await _testSeeder.SeedPredefinedTestsAsync();
                    await transaction.CommitAsync();

                    return Ok("Database populated successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return StatusCode(500, $"There was an error while seeding the database: {ex.Message}, seedFilePath {seedFilePath}, seedFileScoreRangePath {seedFileScoreRangePath}");
                }
            }
        }

        [HttpPost("seed-english")]
        public async Task<IActionResult> SeedTestEnglishAsync()
        {
            var seedFilePath = _testSeeder.ResolveSeedPath("test-otis-english.json");
            var seedFileScoreRangePath = _testSeeder.ResolveSeedPath("classifications-english.json");


            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _testSeeder.SeedTestFromJsonAsync(seedFilePath, seedFileScoreRangePath);
                    await _testSeeder.SeedPredefinedTestsAsync();
                    await transaction.CommitAsync();

                    return Ok("Database populated successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return StatusCode(500, $"There was an error while seeding the database: {ex.Message}, seedFilePath {seedFilePath}, seedFileScoreRangePath {seedFileScoreRangePath}");
                }
            }
        }



    }
}
