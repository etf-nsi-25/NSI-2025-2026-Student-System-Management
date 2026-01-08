using Faculty.Application.DTOs;
using Faculty.Core.Enums;
using FluentValidation;
using System;

namespace Faculty.Application.Validators
{
    public class CreateExamRequestValidator : AbstractValidator<CreateExamRequestDTO>
    {
        public CreateExamRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Exam name is required.")
                .MaximumLength(200).WithMessage("Exam name cannot exceed 200 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Exam location is required.")
                .MaximumLength(200).WithMessage("Exam location cannot exceed 200 characters.");

            RuleFor(x => x.ExamType)
                .IsInEnum().WithMessage($"Exam type must be one of: {string.Join(", ", Enum.GetNames<ExamType>())}.");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("Exam date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Exam date must be in the future.");

            RuleFor(x => x.RegDeadline)
                .NotEmpty().WithMessage("Registration deadline is required.")
                .LessThan(x => x.ExamDate).WithMessage("Registration deadline must be before the exam date.");
        }
    }

    public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequestDTO>
    {
        public UpdateExamRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Exam name is required.")
                .MaximumLength(200).WithMessage("Exam name cannot exceed 200 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Exam location is required.")
                .MaximumLength(200).WithMessage("Exam location cannot exceed 200 characters.");

            RuleFor(x => x.ExamType)
                .IsInEnum().WithMessage($"Exam type must be one of: {string.Join(", ", Enum.GetNames<ExamType>())}.");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("Exam date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Exam date must be in the future.");

            RuleFor(x => x.RegDeadline)
                .NotEmpty().WithMessage("Registration deadline is required.")
                .LessThan(x => x.ExamDate).WithMessage("Registration deadline must be before the exam date.");
        }
    }
}