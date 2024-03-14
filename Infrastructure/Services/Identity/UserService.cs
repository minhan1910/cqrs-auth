using Application.Services.Identity;
using AutoMapper;
using Common.Authorisation;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public UserService(UserManager<ApplicationUser> userManager,
                           RoleManager<ApplicationRole> roleManager,
                           ICurrentUserService currentUserService,
                           IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            var userInDb = await _userManager.FindByIdAsync(changePasswordRequest.UserId);

            if (userInDb is null)
            {
                return await ResponseWrapper.FailAsync("No User was found.");
            }

            bool isCurrentPasswordValid
                = await _userManager.CheckPasswordAsync(userInDb, changePasswordRequest.CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                return await ResponseWrapper.FailAsync("Current password is incorrect.");
            }

            var result = await _userManager.ChangePasswordAsync(userInDb, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

            if (result.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync("Change password successfully!");
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatusRequest)
        {
            var userInDb = await _userManager.FindByIdAsync(changeUserStatusRequest.UserId);

            if (userInDb is null)
            {
                return await ResponseWrapper.FailAsync("No User was found.");
            }

            userInDb.IsActive = changeUserStatusRequest.Activate;

            var result = await _userManager.UpdateAsync(userInDb);

            if (result.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync(changeUserStatusRequest.Activate ?
                    "User activated successfully!" :
                    "User de-activated successfully!");
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> GetRolesAsync(string userId)
        {
            var userInDb = await _userManager.FindByIdAsync(userId);

            if (userInDb is null)
            {
                return await ResponseWrapper.FailAsync("No User was found.");
            }

            var userRolesVm = new List<UserRoleViewModel>();

            // Get all roles
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRoleVm = new UserRoleViewModel
                {
                    RoleDescription = role.Description,
                    RoleName = role.Name,
                    IsAssignedToUser = await _userManager.IsInRoleAsync(userInDb, role.Name)
                };

                userRolesVm.Add(userRoleVm);
            }

            return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRolesVm);
        }

        public async Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email)
        {
            var userInDb = await _userManager.FindByEmailAsync(email);

            if (userInDb is null)
            {
                return await ResponseWrapper<UserResponse>.FailAsync("No user was found.");
            }

            var mappedUser = _mapper.Map<UserResponse>(userInDb);
            return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
        }

        public async Task<IResponseWrapper> GetUserByIdAsync(string Id)
        {
            var userInDb = await _userManager.FindByIdAsync(Id);

            if (userInDb is not null)
            {
                var userResponse = _mapper.Map<UserResponse>(userInDb);
                return await ResponseWrapper<UserResponse>.SuccessAsync(userResponse);
            }

            return await ResponseWrapper.FailAsync("User is not found.");
        }

        public async Task<IResponseWrapper> GetUserListAsync()
        {
            var userList = await _userManager.Users.ToListAsync();

            if (userList.Any())
            {
                var userResponseList = _mapper.Map<List<UserResponse>>(userList);
                return await ResponseWrapper<List<UserResponse>>.SuccessAsync(userResponseList);
            }

            return await ResponseWrapper.FailAsync("No Users were found.");
        }

        public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(userRegistrationRequest.Email);

            if (userWithSameEmail is not null)
            {
                return await ResponseWrapper.FailAsync("User already taken.");
            }

            var userWithSameName = await _userManager.FindByNameAsync(userRegistrationRequest.UserName);

            if (userWithSameName is not null)
            {
                return await ResponseWrapper.FailAsync("Username already taken.");
            }

            var newUser = new ApplicationUser
            {
                Email = userRegistrationRequest.Email,
                FirstName = userRegistrationRequest.FirstName,
                LastName = userRegistrationRequest.LastName,
                PhoneNumber = userRegistrationRequest.PhoneNumber,
                IsActive = userRegistrationRequest.ActiveUser,
                EmailConfirmed = userRegistrationRequest.AutoConfirmEmail,
                UserName = userRegistrationRequest.UserName,
            };

            var result = await _userManager.CreateAsync(newUser, userRegistrationRequest.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

                return await ResponseWrapper.SuccessAsync("User registrated successfully.");
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest)
        {
            var userInDb = await _userManager.FindByIdAsync(updateUserRequest.UserId);

            if (userInDb is null)
            {
                return await ResponseWrapper.FailAsync("No User was found.");
            }

            userInDb.FirstName = updateUserRequest.FirstName;
            userInDb.LastName = updateUserRequest.LastName;
            userInDb.PhoneNumber = updateUserRequest.PhoneNumber;

            var result = await _userManager.UpdateAsync(userInDb);

            if (result.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync("Update userInDb details successfully!");
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest updateUserRoleRequest)
        {
            // cannot un-assign administrator
            // Default admin user seeded by application cannot be assigned/un-assigned

            var userInDb = await _userManager.FindByIdAsync(updateUserRoleRequest.UserId);

            if (userInDb is null)
            {
                return await ResponseWrapper.FailAsync("No User was found.");
            }

            if (userInDb.Email == AppCredentials.Email)
            {
                return await ResponseWrapper.FailAsync("User Roles update not permitted by the same emal.");
            }

            var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);

            if (currentLoggedInUser is null)
            {
                // Maybe deleted, ...
                return await ResponseWrapper.FailAsync("The user logged in was not found.");
            }

            bool currentUserHasAdminRole = await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin);

            if (!currentUserHasAdminRole)
            {
                return await ResponseWrapper.FailAsync("Only admin role can be permitted.");
            }

            // Current user's role updated is Admin Role
            var currentAssingedRoles = await _userManager.GetRolesAsync(userInDb);
           
            var resultRemoveAllUpdateUserRoles = await _userManager.RemoveFromRolesAsync(userInDb, currentAssingedRoles);

            if (resultRemoveAllUpdateUserRoles.Succeeded)
            {
                // add new roles into user
                var allRolesToBeAssigned =
                    updateUserRoleRequest
                   .Roles
                   .Where(role => role.IsAssignedToUser)
                   .Select(role => role.RoleName)
                   .ToArray();

                var resultAddToUserRoles = await _userManager.AddToRolesAsync(userInDb, allRolesToBeAssigned);

                return resultAddToUserRoles.Succeeded ?
                    await ResponseWrapper<string>.SuccessAsync("User roles updated successfully!") :
                    await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(resultAddToUserRoles));
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(resultRemoveAllUpdateUserRoles));
        }

        private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
            => identityResult
               .Errors
               .Select(error => error.Description)
               .ToList();
    }
}