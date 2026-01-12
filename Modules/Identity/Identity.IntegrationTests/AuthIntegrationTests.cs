using Identity.API.DTO.Auth;
using Identity.Core.DTO;
using Identity.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;

namespace Identity.IntegrationTests
{
    public class AuthIntegrationTests : IClassFixture<IdentityApiFactory>, IAsyncLifetime
    {
        private readonly IdentityApiFactory _factory;
        private readonly HttpClient _client;
        private Guid _createdUserId;
        private const string TestEmail = "integration@test.com";
        private const string TestPassword = "Password123!";
        private const string LoginEndpointPath = "/api/auth/login";
        private const string UserEndpointPath = "/api/users";

        public AuthIntegrationTests(IdentityApiFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            // Create a user via API before tests
            var createUserRequest = new CreateUserRequest
            {
                Username = "integrationUser",
                Password = TestPassword,
                FirstName = "Integration",
                LastName = "Test",
                Email = TestEmail,
                FacultyId = Guid.NewGuid(),
                Role = UserRole.Student
            };

            var response = await _client.PostAsJsonAsync(UserEndpointPath, createUserRequest);
            response.EnsureSuccessStatusCode();
            
            var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
            if (userResponse != null)
            {
                _createdUserId = userResponse.Id;
            }
        }

        public async Task DisposeAsync()
        {
            // Delete the user via API after tests
            if (_createdUserId != Guid.Empty)
            {
                var response = await _client.DeleteAsync($"{UserEndpointPath}/{_createdUserId}");
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = TestEmail,
                Password = TestPassword
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);
            // Assert
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
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            errorResponse?.Message.Should().Be("Invalid email or password");
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsEmpty()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = string.Empty,
                Password = TestPassword
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationErrorDetails = await response.Content.ReadFromJsonAsync<ValidationErrorDetails>();
            validationErrorDetails.Should().NotBeNull();
            validationErrorDetails!.Errors["Email"].Should().Contain("Email is required");
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "invalid-email-format",
                Password = TestPassword
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationErrorDetails = await response.Content.ReadFromJsonAsync<ValidationErrorDetails>();
            validationErrorDetails.Should().NotBeNull();
            validationErrorDetails!.Errors["Email"].Should().Contain("Invalid email format");
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = TestEmail,
                Password = string.Empty
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationErrorDetails = await response.Content.ReadFromJsonAsync<ValidationErrorDetails>();
            validationErrorDetails.Should().NotBeNull();
            validationErrorDetails!.Errors["Password"].Should().Contain("Password is required");
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailIsTooLong()
        {
            // Arrange
            var maximumLength = 256;
            var loginRequest = new LoginRequestDto
            {
                Email = new string('a', maximumLength + 1) + "@test.com",
                Password = TestPassword
            };

            // Act
            var response = await _client.PostAsJsonAsync(LoginEndpointPath, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationErrorDetails = await response.Content.ReadFromJsonAsync<ValidationErrorDetails>();
            validationErrorDetails.Should().NotBeNull();
            validationErrorDetails!.Errors["Email"].Should().Contain($"The field Email must be a string with a maximum length of {maximumLength}.");
        }

        private record ErrorResponse(string Message);
        private class ValidationErrorDetails
        {
            public Dictionary<string, string[]> Errors { get; set; } = [];
        }
    }
}
