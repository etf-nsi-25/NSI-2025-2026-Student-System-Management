using Faculty.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Faculty.Application.DTOs
{
    public class ExamDTO
    {
        public int Id { get; set; }
        public Guid CourseId { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? RegDeadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateExamRequestDTO
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public DateTime RegDeadline { get; set; }
    }

    public class UpdateExamRequestDTO
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public DateTime RegDeadline { get; set; }
    }

    public class ExamResponseDTO
    {
        public int Id { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExamType ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? RegDeadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}