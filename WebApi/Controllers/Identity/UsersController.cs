
using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorisation;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class UsersController : MyBaseController<UsersController>
    {
        public UsersController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            var response = await MediatorSender.Send(new UserRegistrationCommand(request));

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserRequest request)
        {
            var response = await MediatorSender.Send(new UserUpdationCommand(request));

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        // api/users/{userId}
        [HttpPost("{userId}")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await MediatorSender.Send(new GetUserByIdQuery(userId));

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        // api/users
        [HttpGet]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserList()
        {
            var response = await MediatorSender.Send(new GetUserListQuery());

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpPut("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var response = await MediatorSender.Send(new ChangeUserPasswordCommand { ChangePassword = changePasswordRequest });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpPut("change-status")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatusRequest)
        {
            var response = await MediatorSender.Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatusRequest });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        #region Roles

        [HttpGet("{userId}/roles")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserRoles([FromRoute] string userId)
        {
            var response = await MediatorSender.Send(new GetUserRolesQuery { UserId = userId });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        [HttpPut("roles")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRoleRequest updateUserRoleRequest)
        {
            var response = await MediatorSender.Send(new UpdateUserRolesCommand { UpdateUserRole = updateUserRoleRequest });

            return response.IsSuccessfull ? Ok(response) : NotFound(response);
        }

        #endregion
    }
}