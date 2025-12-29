using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Support.Application.DTOs;
using Support.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

using EnrollmentRequest = global::Support.Core.Entities.EnrollmentRequest;
using Student = global::Faculty.Core.Entities.Student;
using CreateEnrollmentRequestDTO = global::Support.Application.DTOs.CreateEnrollmentRequestDTO;
using FacultyDbContext = global::Faculty.Infrastructure.Db.FacultyDbContext;

namespace Support.API.IntegrationTests;

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

	private async Task SeedStudentAsync(string userId, Guid facultyId)
	{
		using var scope = _factory.Services.CreateScope();
		var facultyDb = scope.ServiceProvider.GetRequiredService<FacultyDbContext>();

		var exists = await facultyDb.Students.AnyAsync(s => s.UserId == userId);
		if (!exists)
		{
			facultyDb.Students.Add(new Student { UserId = userId, FacultyId = facultyId });
			await facultyDb.SaveChangesAsync();
		}
	}

	private async Task<EnrollmentRequest> CreateEnrollmentAsync(string userId, Guid facultyId, string year = "2024/2025", int semester = 1)
	{
		var dto = new CreateEnrollmentRequestDTO
		{
			UserId = userId,
			FacultyId = facultyId,
			AcademicYear = year,
			Semester = semester
		};

		var response = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto);
		response.StatusCode.Should().Be(HttpStatusCode.Created);

		var created = await response.Content.ReadFromJsonAsync<EnrollmentRequest>();
		created.Should().NotBeNull();
		return created!;
	}

	[Fact]
	public async Task GetEnrollmentRequestById_ShouldReturnOk_WhenExists()
	{
		var userId = "student-getbyid";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);
		var created = await CreateEnrollmentAsync(userId, facultyId);

		var get = await _client.GetAsync($"api/Support/enrollment-requests/{created.Id}");
		get.StatusCode.Should().Be(HttpStatusCode.OK);

		var item = await get.Content.ReadFromJsonAsync<EnrollmentRequest>();
		item.Should().NotBeNull();
		item!.Id.Should().Be(created.Id);
		item.UserId.Should().Be(userId);
		item.Status.Should().Be("Pending");
	}

	[Fact]
	public async Task GetEnrollmentRequestById_ShouldReturnNotFound_WhenDoesNotExist()
	{
		var get = await _client.GetAsync($"api/Support/enrollment-requests/{Guid.NewGuid()}");
		get.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task GetMyEnrollmentRequests_ShouldReturnOnlyUsersItems_AndOrderedByCreatedAtDesc()
	{
		var facultyId = TestTenantService.FacultyId;

		var userA = "student-mine-a";
		var userB = "student-mine-b";

		await SeedStudentAsync(userA, facultyId);
		await SeedStudentAsync(userB, facultyId);

		var a1 = await CreateEnrollmentAsync(userA, facultyId, "2024/2025", 1);
		await Task.Delay(30); 
		var a2 = await CreateEnrollmentAsync(userA, facultyId, "2024/2025", 2);
		var _ = await CreateEnrollmentAsync(userB, facultyId, "2024/2025", 1);

		var resp = await _client.GetAsync($"api/Support/enrollment-requests/my?userId={userA}");
		resp.StatusCode.Should().Be(HttpStatusCode.OK);

		var items = await resp.Content.ReadFromJsonAsync<List<MyEnrollmentItem>>();
		items.Should().NotBeNull();
		items!.Count.Should().Be(2);

		items[0].Id.Should().Be(a2.Id);
		items[1].Id.Should().Be(a1.Id);
		items.All(x => x.Status == "Pending").Should().BeTrue();
	}

	private sealed class MyEnrollmentItem
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public string AcademicYear { get; set; } = default!;
		public int Semester { get; set; }
		public string Status { get; set; } = default!;
	}

	[Fact]
	public async Task ApproveEnrollmentRequest_ShouldReturnOk_AndChangeStatus_WhenPending()
	{
		var userId = "student-approve";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);
		var created = await CreateEnrollmentAsync(userId, facultyId);

		var decide = new DecideEnrollmentRequestDTO
		{
			AdminUserId = "admin-123",
			Note = "Approved in test"
		};

		var resp = await _client.PutAsJsonAsync($"api/Support/enrollment-requests/{created.Id}/approve", decide);
		resp.StatusCode.Should().Be(HttpStatusCode.OK);

		var updated = await resp.Content.ReadFromJsonAsync<EnrollmentRequest>();
		updated.Should().NotBeNull();
		updated!.Status.Should().Be("Approved");
		updated.DecisionAt.Should().NotBeNull();
		updated.DecidedByUserId.Should().Be("admin-123");
		updated.DecisionNote.Should().Be("Approved in test");
	}

	[Fact]
	public async Task RejectEnrollmentRequest_ShouldReturnOk_AndChangeStatus_WhenPending()
	{
		var userId = "student-reject";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);
		var created = await CreateEnrollmentAsync(userId, facultyId, "2024/2025", 1);

		var decide = new DecideEnrollmentRequestDTO
		{
			AdminUserId = "admin-999",
			Note = "Rejected in test"
		};

		var resp = await _client.PutAsJsonAsync($"api/Support/enrollment-requests/{created.Id}/reject", decide);
		resp.StatusCode.Should().Be(HttpStatusCode.OK);

		var updated = await resp.Content.ReadFromJsonAsync<EnrollmentRequest>();
		updated.Should().NotBeNull();
		updated!.Status.Should().Be("Rejected");
		updated.DecisionAt.Should().NotBeNull();
		updated.DecidedByUserId.Should().Be("admin-999");
		updated.DecisionNote.Should().Be("Rejected in test");
	}

	[Fact]
	public async Task ApproveEnrollmentRequest_ShouldReturnBadRequest_WhenNotPending()
	{
		var userId = "student-approve-notpending";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);
		var created = await CreateEnrollmentAsync(userId, facultyId);

		var first = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{created.Id}/approve",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "first" });

		first.StatusCode.Should().Be(HttpStatusCode.OK);

		var second = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{created.Id}/approve",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "second" });

		second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task RejectEnrollmentRequest_ShouldReturnBadRequest_WhenNotPending()
	{
		var userId = "student-reject-notpending";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);
		var created = await CreateEnrollmentAsync(userId, facultyId);

		var first = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{created.Id}/reject",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "first" });

		first.StatusCode.Should().Be(HttpStatusCode.OK);

		var second = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{created.Id}/reject",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "second" });

		second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task ApproveEnrollmentRequest_ShouldReturnNotFound_WhenIdDoesNotExist()
	{
		var resp = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{Guid.NewGuid()}/approve",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "x" });

		resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task RejectEnrollmentRequest_ShouldReturnNotFound_WhenIdDoesNotExist()
	{
		var resp = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{Guid.NewGuid()}/reject",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "x" });

		resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task CreateEnrollmentRequest_ShouldReturnBadRequest_WhenSemesterIsInvalid()
	{
		var userId = "student-invalid-semester";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);

		var dto = new CreateEnrollmentRequestDTO
		{
			UserId = userId,
			FacultyId = facultyId,
			AcademicYear = "2024/2025",
			Semester = 3 
		};

		var resp = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto);
		resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task CreateEnrollmentRequest_ShouldReturnBadRequest_WhenAcademicYearIsEmpty()
	{
		var userId = "student-empty-year";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);

		var dto = new CreateEnrollmentRequestDTO
		{
			UserId = userId,
			FacultyId = facultyId,
			AcademicYear = "   ",
			Semester = 1
		};

		var resp = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto);
		resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task CreateEnrollmentRequest_ShouldReturnBadRequest_WhenDuplicateExists_AndNotRejected()
	{
		var userId = "student-dup";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);

		var created = await CreateEnrollmentAsync(userId, facultyId, "2024/2025", 1);
		created.Status.Should().Be("Pending");

		var dto2 = new CreateEnrollmentRequestDTO
		{
			UserId = userId,
			FacultyId = facultyId,
			AcademicYear = "2024/2025",
			Semester = 1
		};

		var resp2 = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto2);
		resp2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task CreateEnrollmentRequest_ShouldAllowNewRequest_WhenPreviousWasRejected()
	{
		var userId = "student-rejected-then-new";
		var facultyId = TestTenantService.FacultyId;

		await SeedStudentAsync(userId, facultyId);

		var first = await CreateEnrollmentAsync(userId, facultyId, "2024/2025", 1);

		var rejectResp = await _client.PutAsJsonAsync(
			$"api/Support/enrollment-requests/{first.Id}/reject",
			new DecideEnrollmentRequestDTO { AdminUserId = "admin", Note = "rejecting" });

		rejectResp.StatusCode.Should().Be(HttpStatusCode.OK);

		var dto2 = new CreateEnrollmentRequestDTO
		{
			UserId = userId,
			FacultyId = facultyId,
			AcademicYear = "2024/2025",
			Semester = 1
		};

		var resp2 = await _client.PostAsJsonAsync("api/Support/enrollment-requests", dto2);
		resp2.StatusCode.Should().Be(HttpStatusCode.Created);
	}
}
