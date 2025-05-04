namespace ExamiNation.Application.DTOs.ScoreRange
{
    public class ScoreRangeDto
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }
        public string TestName { get; set; }

        public int MinScore { get; set; }

        public int MaxScore { get; set; }

        public string Classification { get; set; }
    }
}
