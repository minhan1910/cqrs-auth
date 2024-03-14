using Common.Responses.Identity;

namespace Common.Requests.Identity
{
    public class UpdateUserRoleRequest
    {
        public string UserId { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
    }
}