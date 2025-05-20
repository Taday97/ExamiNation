using AutoMapper;
using ExamiNation.Application.DTOs.Question;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Mapping.Resolvers
{
    public class SelectedOptionResolver : IValueResolver<Question, QuestionDtoWithOptions, Guid?>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SelectedOptionResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? Resolve(Question source, QuestionDtoWithOptions destination, Guid? destMember, ResolutionContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return null;

            var answers = source.Answers as ICollection<Answer>;
            if (answers == null)
                return null;

            var selectedAnswer = answers.FirstOrDefault(a => a.QuestionId == source.Id && a.TestResult != null && a.TestResult.Status != TestResultStatus.Completed && a.TestResult.UserId == userId);
            return selectedAnswer?.OptionId;
        }
    }
}
