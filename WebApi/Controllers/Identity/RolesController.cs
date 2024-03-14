using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Queries;
using Common.Authorisation;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class RolesController : MyBaseController<RolesController>
    {
        public RolesController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        [HttpPost]
        [MustHavePermission(AppFeature.Roles, AppAction.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var response = await MediatorSender.Send(new CreateRoleCommand { CreateRoleRequest = request });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpGet("{roleId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var response = await MediatorSender.Send(new GetRoleByIdQuery { RoleId = roleId });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles()
        {
            var response = await MediatorSender.Send(new GetRolesQuery());

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateRole(UpdateRoleRequest updateRoleRequest)
        {
            var response = await MediatorSender.Send(new UpdateRoleCommand { UpdateRoleRequest = updateRoleRequest });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{roleId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Delete)]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await MediatorSender.Send(new DeleteRoleCommand { RoleId = roleId });

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        #region RoleClaims (Permissions)

        [HttpGet("{roleId}/permissions")]
        [MustHavePermission(AppFeature.RoleClaims, AppAction.Read)]
        public async Task<IActionResult> GetAllPermissions([FromRoute] string roleId)
        {
            var response = await MediatorSender.Send(new GetAllPermissionsQuery { RoleId = roleId } );

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpPut("update-permissions")]
        [MustHavePermission(AppFeature.RoleClaims, AppAction.Update)]
        public async Task<IActionResult> UpdatePermissions([FromBody] UpdateRolePermissionRequest updateRolePermissionRequest)
        {
            var response = await MediatorSender.Send(new UpdateRolePermissionsCommand 
            { 
                UpdateRolePermissionRequest = updateRolePermissionRequest
            });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        #endregion
    }
}
