using Identity.API.DTO.Auth;
using Identity.Core.DTO;
using Identity.Core.Enums;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;


namespace Identity.IntegrationTests
{
    public class AuthIntegrationTests : IClassFixture<IdentityApiFactory>, IAsyncLifetime
    {
        private readonly IdentityApiFactory _factory;
        private readonly HttpClient _client;
        private const string TestEmail = "integration@test.com";
        private const string TestPassword = "Password123!";
        private const string LoginEndpointPath = "/api/auth/login";

        public AuthIntegrationTests(IdentityApiFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var existingUser = await userManager.FindByEmailAsync(TestEmail);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = TestEmail,
                    Email = TestEmail,
                    FirstName = "Integration",
                    LastName = "Test",
                    EmailConfirmed = true,
                    Role = UserRole.Student,
                    FacultyId = Guid.NewGuid(),
                    Status = UserStatus.Active
                };

                var result = await userManager.CreateAsync(user, TestPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Seed user creation failed: {errors}");
                }
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequestDto 
            { 
                Email = TestEmail, 
                Password = TestPassword 
            };

            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResponse.Should().NotBeNull();
            loginResponse!.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(TestEmail, "WrongPassword")]
        [InlineData("nonexistentemail@test.com", "AnyPassword")]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid(string email, string password)
        {
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsEmpty()
        {
            var loginRequest = new LoginRequestDto
            {
                Email = string.Empty,
                Password = TestPassword
            };

            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            var loginRequest = new LoginRequestDto
            {
                Email = "invalid-email-format",
                Password = TestPassword
            };

            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private record ErrorResponse(string Message);
        
        private class ValidationErrorDetails
        {
            public Dictionary<string, string[]> Errors { get; set; } = [];
        }
    }
}