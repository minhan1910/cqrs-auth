using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands
{
    public sealed record UserRegistrationCommand(UserRegistrationRequest UserRegistrationRequest) : IRequest<IResponseWrapper>
    {
    }

    public class UserRegistrationCommandHandler : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
    {
        private readonly IUserService _userSerivce;

        public UserRegistrationCommandHandler(IUserService userSerivce)
        {
            _userSerivce = userSerivce;
        }

        public async Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            return await _userSerivce.RegisterUserAsync(request.UserRegistrationRequest);
        }
    }
}