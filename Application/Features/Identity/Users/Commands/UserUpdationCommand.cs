using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands
{
    public sealed record UserUpdationCommand(UpdateUserRequest UpdateUserRequest) : IRequest<IResponseWrapper>
    {
    }

    public class UserUpdationCommandHandler : IRequestHandler<UserUpdationCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public UserUpdationCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(UserUpdationCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserAsync(request.UpdateUserRequest);
        }
    }
}