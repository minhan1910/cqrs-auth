using Application.Services.Identity;
using AutoMapper;
using Common.Authorisation;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<ApplicationRole> roleManager, 
                           UserManager<ApplicationUser> userManager,
                           IMapper mapper,
                           ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest)
        {
            var roleExists = await _roleManager.FindByNameAsync(createRoleRequest.RoleName);

            if (roleExists is not null)
            {
                return await ResponseWrapper.FailAsync("Role is taken.");
            }

            var newRole = new ApplicationRole
            {
                Name = createRoleRequest.RoleName,
                Description = createRoleRequest.Description,
            };

            var result = await _roleManager.CreateAsync(newRole);

            return result.Succeeded ?
                await ResponseWrapper<string>.SuccessAsync("Create role succesfully!") :
                await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> DeleteRoleByIdAsync(string roleId)
        {
            var roleExists = await _roleManager.FindByIdAsync(roleId);

            if ( roleExists is null)
            {
                return await ResponseWrapper.FailAsync("No Role was found!");
            }

            if (roleExists.Name == AppRoles.Admin)
            {
                return await ResponseWrapper.FailAsync("Admin role couldn't delete.");
            }

            // check: is user assined to role -> not deleting
            var allUsers = await _userManager.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, roleExists.Name))
                {
                    return await ResponseWrapper.FailAsync($"Role: {roleExists.Name} is currently assigned to user.");
                }
            }

            // Delete role
            var result = await _roleManager.DeleteAsync(roleExists);

            return result.Succeeded ?
               await ResponseWrapper<string>.SuccessAsync("Role deleted successfully!") :
               await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);

            if (roleInDb is null)
            {
                return await ResponseWrapper.FailAsync("No Role was found.");
            }

            var roleClaimResponse = new RoleClaimResponse
            {
                Role = new()
                {
                    Id = roleId,
                    RoleName = roleInDb.Name,
                    Description = roleInDb.Description,
                },
                RoleClaims = new()
            };

            var allPermissionsCached = AppPermissions.AllPermissions;
            // get only claims (permissions) assigned for specific current role
            var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);

            var allPermissionNames = allPermissionsCached.Select(p => p.Name);
            var currentRoleClaimsValues = currentRoleClaims.Select(crc => crc.ClaimValue);

            var currentlyAssignedRoleClaimsNames = 
                allPermissionNames
                .Intersect(currentRoleClaimsValues)
                .ToHashSet();

            foreach (var permission in allPermissionsCached)
            {
                roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                {
                    RoleId = roleId,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group,
                    IsAssginedToRole = currentlyAssignedRoleClaimsNames.Contains(permission.Name)
                });
            }

            return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }        

        private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
        {
            var result = await 
                _context
                .RoleClaims
                .Where(roleClaim => roleClaim.RoleId == roleId)
                .ToListAsync();

            if (result.Any())
            {
                var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(result);
                return mappedRoleClaims;
            }

            return new();
        }

        public async Task<IResponseWrapper> GetRoleByIdAsync(string roleId)
        {
            var roleExists = await _roleManager.FindByIdAsync(roleId);

            if (roleExists is not null)
            {
                var roleResponse = _mapper.Map<RoleResponse>(roleExists);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(roleResponse);
            }

            return await ResponseWrapper.FailAsync("No Role was found!");
        }

        public async Task<IResponseWrapper> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles.Any())
            {
                var roleReponses = _mapper.Map<List<RoleResponse>>(roles);
                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(roleReponses);
            }

            return await ResponseWrapper.FailAsync("Now roles were found.");
        }

        public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest)
        {
            var roleExists = await _roleManager.FindByIdAsync(updateRoleRequest.RoleId);

            if (roleExists is null)
            {
                return await ResponseWrapper.FailAsync("No role was found.");
            }

            if (roleExists.Name == AppRoles.Admin)
            {
                return await ResponseWrapper.FailAsync("Admin role couldn't update.");
            }

            roleExists.Name = updateRoleRequest.RoleName;
            roleExists.Description = updateRoleRequest.RoleDescription;
            var result = await _roleManager.UpdateAsync(roleExists);

            return result.Succeeded ?
                await ResponseWrapper<string>.SuccessAsync("Role updated successfully!") :
                await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }

        public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionRequest updateRolePermissionRequest)
        {
            var roleExists = await _roleManager.FindByIdAsync(updateRolePermissionRequest.RoleId);

            if (roleExists is null)
            {
                return await ResponseWrapper.FailAsync("No role was found.");
            }

            if (roleExists.Name == AppRoles.Admin)
            {
                return await ResponseWrapper.FailAsync("Cannot change permission of Admin role.");
            }

            var permissionsToBeAssignedFromRequest =
                updateRolePermissionRequest
                .RoleClaims 
                .Where(rc => rc.IsAssginedToRole)
                .ToList();

            var currentlyAssignedClaims = await _roleManager.GetClaimsAsync(roleExists);

            // Remove all old assigned claims
            //var removeClaimsTask = 
            //    currentlyAssignedClaims                
            //    .Select(claim => _roleManager.RemoveClaimAsync(roleExists, claim));

            //await Task.WhenAll(removeClaimsTask);

            foreach (var claim in currentlyAssignedClaims)
            {
                await _roleManager.RemoveClaimAsync(roleExists, claim);
            }

            // Update new claims
            var mappedPermissionsRequestToApplicationRoleClaim =
                _mapper.Map<List<ApplicationRoleClaim>>(permissionsToBeAssignedFromRequest);

            foreach (var permission in mappedPermissionsRequestToApplicationRoleClaim)
            {
                await _context.RoleClaims.AddAsync(permission);
            }
            await _context.SaveChangesAsync();


            return await ResponseWrapper<string>.SuccessAsync("Role permission updated");
        }

        private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
           => identityResult
              .Errors
              .Select(error => error.Description)
              .ToList();

    }
}