using System.Runtime.CompilerServices;
using Identity.Core.Entities;
using Identity.Core.Repositories;
using Identity.Core.Events;
using Identity.Core.Enums;
using Identity.Core.DTO; 
using Identity.Core.Services;

// Allow injection.
[assembly: InternalsVisibleTo("Identity.Infrastructure")]

namespace Identity.Application.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IIdentityHasherService identityHasherService;
        private readonly IEventPublisher eventPublisher;

        public UserService(
            IUserRepository userRepository,
            IIdentityHasherService identityHasherService,
            IEventPublisher eventPublisher)
        {
            this.userRepository = userRepository;
            this.identityHasherService = identityHasherService;
            this.eventPublisher = eventPublisher;
        }

        public async Task<Guid> CreateUserAsync(
            string username,
            string password,
            string firstName,
            string lastName,
            Guid facultyId,
            string? indexNumber,
            UserRole role)
        {
            if (role == UserRole.Superadmin || role == UserRole.Admin)
            {
                throw new InvalidOperationException("Admin users are restricted from assigning Superadmin or Admin roles.");
            }

            if (await userRepository.IsUsernameTakenAsync(username))
            {
                throw new ArgumentException($"Username '{username}' is already taken.", nameof(username));
            }

            var passwordHash = identityHasherService.HashPassword(password);

            var newUser = User.Create(
                username,
                passwordHash,
                firstName,
                lastName,
                facultyId,
                role,
                indexNumber
            );

            await userRepository.AddAsync(newUser);
            await userRepository.SaveAsync();

            var userCreatedEvent = new UserCreatedEvent(
                newUser.Id,
                newUser.Username,
                newUser.FirstName,
                newUser.LastName,
                newUser.FacultyId,
                newUser.Role,
                newUser.IndexNumber
            );

            await eventPublisher.PublishAsync(userCreatedEvent);

            return newUser.Id;
        }

        public async Task<UserListResponse> GetAllUsersAsync(UserFilterRequest filter)
        {
            var domainUsers = await userRepository.GetAllFilteredAsync(filter);
            var totalCount = await userRepository.CountAsync(filter);

            var userResponses = domainUsers.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FacultyId = u.FacultyId,
                IndexNumber = u.IndexNumber,
                Role = u.Role,
                Status = u.Status
            }).ToList();

            return new UserListResponse
            {
                Items = userResponses,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FacultyId = user.FacultyId,
                Status = user.Status,
                IndexNumber = user.IndexNumber,
                Role = user.Role
            };
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (user.Role == UserRole.Superadmin)
                throw new InvalidOperationException("Cannot delete Superadmin user.");

            await userRepository.DeleteAsync(user);
            await userRepository.SaveAsync();

            await eventPublisher.PublishAsync(new UserDeletedEvent(userId));

            return true;
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserRequest request)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            var previousRole = user.Role;

            if (request.Role == UserRole.Superadmin || request.Role == UserRole.Admin)
                throw new InvalidOperationException("Admin users are restricted from assigning Superadmin or Admin roles.");

            user.UpdateDetails(
                request.FirstName,
                request.LastName,
                request.FacultyId,
                request.IndexNumber
            );

            user.ChangeRole(request.Role);
            user.ChangeStatus(request.Status);

            await userRepository.UpdateAsync(user);
            await userRepository.SaveAsync();

            if (previousRole != user.Role)
            {
                var roleAssignedEvent = new UserRoleAssignedEvent(userId, previousRole, user.Role);
                await eventPublisher.PublishAsync(roleAssignedEvent);
            }

            return true;
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (user.Role == UserRole.Superadmin)
                throw new InvalidOperationException("Cannot deactivate Superadmin user.");

            user.ChangeStatus(UserStatus.Inactive);

            await userRepository.UpdateAsync(user);
            await userRepository.SaveAsync();

            return true;
        }
    }
}
