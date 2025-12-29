using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using EnrollmentRequest = global::Support.Core.Entities.EnrollmentRequest;
using Student = global::Faculty.Core.Entities.Student;
using CreateEnrollmentRequestDTO = global::Support.Application.DTOs.CreateEnrollmentRequestDTO;
using FacultyDbContext = global::Faculty.Infrastructure.Db.FacultyDbContext;

namespace Support.API.IntegrationTests
{
	public class EnrollmentRequestsTests : IClassFixture<SupportApiFactory>
	{
		private readonly HttpClient _client;
		private readonly SupportApiFactory _factory;

		public EnrollmentRequestsTests(SupportApiFactory factory)
		{
			_factory = factory;
			_client = factory.CreateClient();

			_client.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
		}

		[Fact]
		public async Task CreateEnrollmentRequest_ShouldReturnCreated_WhenDataIsValid()
		{
			var testUserId = "student-123";

			using (var scope = _factory.Services.CreateScope())
			{
				var facultyDb = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

				facultyDb.Students.Add(new Student { UserId = testUserId });
				await facultyDb.SaveChangesAsync();
			}

			var dto = new CreateEnrollmentRequestDTO
			{
				UserId = testUserId,
				FacultyId = Guid.NewGuid(),
				AcademicYear = "2024/2025",
				Semester = 1
			};

			var response = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto);

			response.StatusCode.Should().Be(HttpStatusCode.Created);

			var result = await response.Content.ReadFromJsonAsync<EnrollmentRequest>();
			result.Should().NotBeNull();
			result!.UserId.Should().Be(testUserId);
			result.Status.Should().Be("Pending");
		}

		[Fact]
		public async Task CreateEnrollmentRequest_ShouldReturnBadRequest_WhenStudentDoesNotExist()
		{
			var dto = new CreateEnrollmentRequestDTO
			{
				UserId = "nepostojeci-student",
				FacultyId = Guid.NewGuid(),
				AcademicYear = "2024/2025",
				Semester = 1
			};

			var response = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}
	}
}