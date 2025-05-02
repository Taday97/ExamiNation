﻿using ExamiNation.Application.DTOs.Role;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.Answer
{

    public class EditAnswerDtoValidator : AbstractValidator<AnswerDto>
    {
        public EditAnswerDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Answer ID is required.")
                .Must(BeAValidGuid).WithMessage("Answer ID must be a valid GUID.");

            RuleFor(x => x.TestResultId)
                .NotEmpty().WithMessage("TestResult ID is required.")
                .Must(BeAValidGuid).WithMessage("TestResult ID must be a valid GUID.");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("Question ID is required.")
                .Must(BeAValidGuid).WithMessage("Question ID must be a valid GUID.");

            RuleFor(x => x.QuestionText)
                .MaximumLength(1000).WithMessage("Question Text cannot exceed 1000 characters.");

            RuleFor(x => x.OptionId)
                .Must(BeAValidGuid).When(x => x.OptionId.HasValue).WithMessage("Option ID must be a valid GUID.");

            RuleFor(x => x.OptionText)
                .MaximumLength(1000).WithMessage("Option Text cannot exceed 1000 characters.");

            RuleFor(x => x.Text)
                .MaximumLength(1000).WithMessage("Text cannot exceed 1000 characters.");
        }

        private bool BeAValidGuid(Guid? id)
        {
            return id != Guid.Empty;
        }

        private bool BeAValidGuid(Guid id)
        {
            return id != Guid.Empty;
        }
    }

}
