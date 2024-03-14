using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IRoleService
    { 
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);

        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest);

        Task<IResponseWrapper> GetRolesAsync();

        Task<IResponseWrapper> GetRoleByIdAsync(string roleId);

        Task<IResponseWrapper> DeleteRoleByIdAsync(string roleId);

        Task<IResponseWrapper> GetPermissionsAsync(string roleId);

        Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionRequest updateRolePermissionRequest);
    }
}