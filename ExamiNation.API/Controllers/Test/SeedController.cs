using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using ExamiNation.Infrastructure.Data.Seed;
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
            var testFilePath = "..\\ExamiNation.Infrastructure\\Data\\Seed\\json\\test-otis.json";
            var scoreRangeFilePath = "..\\ExamiNation.Infrastructure\\Data\\Seed\\json\\classifications.json";

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _testSeeder.SeedTestFromJsonAsync(testFilePath, scoreRangeFilePath);
                    await transaction.CommitAsync();

                    return Ok("Database populated successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return StatusCode(500, $"There was an error while seeding the database: {ex.Message}");
                }
            }
        }

        [HttpPost("seed-english")]
        public async Task<IActionResult> SeedTestEnglishAsync()
        {
            var testFilePath = "..\\ExamiNation.Infrastructure\\Data\\Seed\\json\\test-otis-english.json"; 
            var scoreRangeFilePath = "..\\ExamiNation.Infrastructure\\Data\\Seed\\json\\classifications-english.json"; 

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _testSeeder.SeedTestFromJsonAsync(testFilePath, scoreRangeFilePath);
                    await transaction.CommitAsync();

                    return Ok("Database populated successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return StatusCode(500, $"There was an error while seeding the database: {ex.Message}");
                }
            }
        }



    }
}
