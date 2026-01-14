using Faculty.Application.DTOs;
using Faculty.Application.Validators;
using Faculty.Core.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace Faculty.Test;

public class ExamValidatorsTests
{
    private readonly CreateExamRequestValidator _createValidator;
    private readonly UpdateExamRequestValidator _updateValidator;

    public ExamValidatorsTests()
    {
        _createValidator = new CreateExamRequestValidator();
        _updateValidator = new UpdateExamRequestValidator();
    }

    [Fact]
    public void CreateExamRequestValidator_ValidRequest_ShouldPassValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateExamRequestValidator_EmptyName_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "",
            Location = "Room 101",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Exam name is required.");
    }

    [Fact]
    public void CreateExamRequestValidator_NameTooLong_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = new string('A', 201), // 201 characters
            Location = "Room 101",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Exam name cannot exceed 200 characters.");
    }

    [Fact]
    public void CreateExamRequestValidator_EmptyLocation_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
              .WithErrorMessage("Exam location is required.");
    }

    [Fact]
    public void CreateExamRequestValidator_InvalidExamType_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = (ExamType)0,
            ExamDate = DateTime.UtcNow.AddDays(7),
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExamType)
              .WithErrorMessage("Exam type must be one of: Written, Oral, Practical, Online.");
    }

    [Fact]
    public void CreateExamRequestValidator_PastExamDate_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(-1), // Past date
            RegDeadline = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExamDate)
              .WithErrorMessage("Exam date must be in the future.");
    }

    [Fact]
    public void CreateExamRequestValidator_RegDeadlineAfterExamDate_ShouldFailValidation()
    {
        // Arrange
        var request = new CreateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Final Exam",
            Location = "Room 101",
            ExamType = ExamType.Written,
            ExamDate = DateTime.UtcNow.AddDays(5),
            RegDeadline = DateTime.UtcNow.AddDays(7) // After exam date
        };

        // Act
        var result = _createValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RegDeadline)
              .WithErrorMessage("Registration deadline must be before the exam date.");
    }

    [Fact]
    public void UpdateExamRequestValidator_ValidRequest_ShouldPassValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateExamRequestValidator_EmptyName_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "",
            Location = "Room 202",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Exam name is required.");
    }

    [Fact]
    public void UpdateExamRequestValidator_EmptyLocation_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
              .WithErrorMessage("Exam location is required.");
    }

    [Fact]
    public void UpdateExamRequestValidator_InvalidExamType_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = (ExamType)0,
            ExamDate = DateTime.UtcNow.AddDays(10),
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
          result.ShouldHaveValidationErrorFor(x => x.ExamType)
              .WithErrorMessage("Exam type must be one of: Written, Oral, Practical, Online.");
    }

    [Fact]
    public void UpdateExamRequestValidator_PastExamDate_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(-1), // Past date
            RegDeadline = DateTime.UtcNow.AddDays(8)
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExamDate)
              .WithErrorMessage("Exam date must be in the future.");
    }

    [Fact]
    public void UpdateExamRequestValidator_RegDeadlineAfterExamDate_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateExamRequestDTO
        {
            CourseId = Guid.NewGuid(),
            Name = "Updated Exam",
            Location = "Room 202",
            ExamType = ExamType.Oral,
            ExamDate = DateTime.UtcNow.AddDays(5),
            RegDeadline = DateTime.UtcNow.AddDays(7) // After exam date
        };

        // Act
        var result = _updateValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RegDeadline)
              .WithErrorMessage("Registration deadline must be before the exam date.");
    }
}