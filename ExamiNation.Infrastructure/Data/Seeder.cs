using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Repositories.Test;
using System.Text.Json;

namespace ExamiNation.Infrastructure.Data
{
    public class Seeder
    {
        private readonly ITestRepository _testRepository;
        private readonly ICognitiveCategoryRepository _cognitiveCategoryRepository;
        private readonly IScoreRangeRepository _scoreRangeRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IOptionRepository _optionRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITestResultRepository _testResultRepository;

        public Seeder(ITestRepository testRepository, ICognitiveCategoryRepository cognitiveCategoryRepository, IScoreRangeRepository scoreRangeRepository, IAnswerRepository answerRepository, IOptionRepository optionRepository, IQuestionRepository questionRepository, ITestResultRepository testResultRepository)
        {
            _testRepository = testRepository;
            _cognitiveCategoryRepository = cognitiveCategoryRepository;
            _scoreRangeRepository = scoreRangeRepository;
            _answerRepository = answerRepository;
            _optionRepository = optionRepository;
            _questionRepository = questionRepository;
            _testResultRepository = testResultRepository;
        }

        public async Task SeedTestFromJsonAsync(string filePath, string filePathClassification)
        {
            var query = new Domain.Common.QueryOptions<Test>
            {
                AsNoTracking = false
            };
            query.Includes.Add(t => t.TestResults);
            query.Includes.Add(t => t.Questions);

            var existingTests = await _testRepository.GetAllAsync(query);
            var congniteCategories = await _cognitiveCategoryRepository.GetAllAsync();

            foreach (var existingTest in existingTests)
            {
                try
                {
                    foreach (var testResult in existingTest.TestResults)
                    {
                        var answers = await _answerRepository.GetByIdAsync(testResult.Id);
                        if (answers != null)
                            await _answerRepository.DeleteAsync(answers.Id);

                        await _testResultRepository.DeleteAsync(testResult.Id);
                    }

                    foreach (var question in existingTest.Questions)
                    {
                        var options = await _optionRepository.GetByIdAsync(question.Id);
                        if (options != null)
                            await _optionRepository.DeleteAsync(options.Id);

                        await _questionRepository.DeleteAsync(question.Id);
                    }

                    await _testRepository.DeleteAsync(existingTest.Id);

                    foreach (var category in congniteCategories)
                    {
                        await _cognitiveCategoryRepository.DeleteAsync(category.Id);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
           
            await GetPredefinedIQCognitiveCategories();

            var cognitiveCategories = await _cognitiveCategoryRepository.GetAllAsync();
            var listCognitiveCategories= cognitiveCategories.Where(c => c.TestTypeId == (int)TestType.IQ).ToList();

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
                ImageUrl = "uploads/test/a14ada58c7aba0481349f0ec6476d63d.jpg",
                Questions = dto.Questions.Select(q =>
                {
                    return new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = q.Text,
                        QuestionNumber = q.QuestionNumber,
                        Type = q.Type,
                        CognitiveCategoryId = listCognitiveCategories.FirstOrDefault(l=>l.Code==q.CognitiveCategoryCode)?.Id,
                        Options = q.Options?.Select(o => new Option
                        {
                            Id = Guid.NewGuid(),
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    };
                }).ToList()
               
            };

            await _testRepository.AddAsync(test);


            await SeedClassificationsFromJsonAsync(filePathClassification, test);

        }

        public async Task SeedClassificationsFromJsonAsync(string filePath, Test test)
        {


            var existingClassifications = await _scoreRangeRepository.GetAllAsync();
            foreach (var classification in existingClassifications)
            {
                await _scoreRangeRepository.DeleteAsync(classification.Id);
            }


            var json = await File.ReadAllTextAsync(filePath);
            var classifications = JsonSerializer.Deserialize<List<ScoreRange>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            foreach (var classification in classifications)
            {
                classification.Test = test;
                await _scoreRangeRepository.AddAsync(classification);
            }
        }

        public string ResolveSeedPath(string filename)
        {
            var dockerPath = Path.Combine(AppContext.BaseDirectory, filename);

            if (File.Exists(dockerPath))
                return dockerPath;

            var localPath = Path.Combine("Seed", filename);
            if (File.Exists(localPath))
                return localPath;

            throw new FileNotFoundException($"Seed file '{filename}' not found in Docker or local path.");
        }

        public async Task SeedPredefinedTestsAsync()
        {
            var predefinedTests = new List<Test>
    {
        new Test
        {
            Name = "Comprehensive IQ Assessment",
            Description = "Evaluate your cognitive abilities across multiple dimensions including logical reasoning, pattern recognition, and spatial awareness.",
            Type = TestType.IQ,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/a14ada58c7aba0481349f0ec6476d63d.jpg",
            Questions = new List<Question>()

        },
        new Test
        {
            Name = "Big Five Personality Test",
            Description = "Discover your personality traits across five dimensions: openness, conscientiousness, extraversion, agreeableness, and neuroticism.",
            Type = TestType.Personality,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/1615d0fb9299d9baef80ea3db8535612.jpg",
            Questions = new List<Question>()
        },
        new Test
        {
            Name = "Career Path Finder",
            Description = "Identify your professional strengths, interests, and values to discover career paths that align with your natural talents and preferences.",
            Type = TestType.Skills,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/6921a2fbb8344565d10f6c1df8abfa28.jpg",
            Questions = new List<Question>()
        },
        new Test
        {

            Name = "Emotional Intelligence Assessment",
            Description = "Measure your ability to recognize, understand and manage emotions in yourself and others, a key factor in personal and professional success.",
            Type = TestType.Other,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/823394bee9086a231ed455ae1a53e067.jpg",
            Questions = new List<Question>()
        },
        new Test
        {
            Name = "Leadership Style Assessment",
            Description = "Identify your natural leadership approach and learn how to leverage your strengths to effectively guide teams and inspire others.",
            Type = TestType.Skills,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/d9c496f27db6d3a02aab446987c1a118.jpg",
            Questions = new List<Question>()
        },
        new Test
        {
            Name = "Learning Style Identifier",
            Description = "Discover whether you're a visual, auditory, reading/writing, or kinesthetic learner to optimize your learning strategies and improve retention.",
            Type = TestType.Skills,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = "uploads/test/4e80daca615db77cd5925c0a8a4cde6e.jpg",
            Questions = new List<Question>()
        }
    };

            foreach (var test in predefinedTests)
            {
                await _testRepository.AddAsync(test);
            }

        }

        public async Task GetPredefinedIQCognitiveCategories()
        {
            var predefinedCategories = new List<CognitiveCategory>
    {
        new CognitiveCategory
        {
            Name = "Logical Reasoning",
            Code = "LOGIC",
            Description = "Ability to analyze patterns, sequences, and relationships logically.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Mathematical Ability",
            Code = "MATH",
            Description = "Skill in understanding and working with numbers and mathematical concepts.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Verbal Reasoning",
            Code = "VERBAL",
            Description = "Capability to understand and reason using concepts framed in words.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Spatial Awareness",
            Code = "SPATIAL",
            Description = "Ability to visualize and manipulate objects in space.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Memory",
            Code = "MEMORY",
            Description = "Capacity to store, retain, and recall information effectively.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Processing Speed",
            Code = "SPEED",
            Description = "Speed at which cognitive tasks are performed.",
            TestTypeId = (int)TestType.IQ
        },
        new CognitiveCategory
        {
            Name = "Attention to Detail",
            Code = "ATTENTION",
            Description = "Ability to focus and notice small details accurately.",
            TestTypeId = (int)TestType.IQ
        }
    };
            foreach (var category in predefinedCategories)
            {
                await _cognitiveCategoryRepository.AddAsync(category);
            }
        }

    }

}
