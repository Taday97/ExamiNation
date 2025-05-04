using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using System.Text.Json;

namespace ExamiNation.Infrastructure.Data.Seed
{
    public class Seeder
    {
        private readonly ITestRepository _testRepository;
        private readonly IScoreRangeRepository _scoreRangeRepository;

        public Seeder(ITestRepository testRepository, IScoreRangeRepository scoreRangeRepository)
        {
            _testRepository = testRepository;
            _scoreRangeRepository = scoreRangeRepository;
        }

        public async Task SeedTestFromJsonAsync(string filePath, string filePathClassification)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var dto = JsonSerializer.Deserialize<Test>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var test = new Test
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                Questions = dto.Questions.Select(q => new Question
                {
                    Id = Guid.NewGuid(),
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options?.Select(o => new Option
                    {
                        Id = Guid.NewGuid(),
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };

            await _testRepository.AddAsync(test);

            await  SeedClassificationsFromJsonAsync(filePathClassification,test);

        }

        public async Task SeedClassificationsFromJsonAsync(string filePath, Test test)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var classifications = JsonSerializer.Deserialize<List<ScoreRange>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            foreach (var classification in classifications)
            {
                classification.Test= test;
                await _scoreRangeRepository.AddAsync(classification);
            }
        }
    }

}
