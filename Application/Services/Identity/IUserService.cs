using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email);

        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest);

        Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest);

        Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePasswordRequest);

        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatusRequest);

        Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest updateUserRoleRequest);

        Task<IResponseWrapper> GetUserByIdAsync(string Id);

        Task<IResponseWrapper> GetUserListAsync();

        Task<IResponseWrapper> GetRolesAsync(string userId);
    }
}