using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Queries
{
    public sealed record GetUserListQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, IResponseWrapper>
    {
        private readonly IUserService _userService;

        public GetUserListQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserListAsync();
        }
    }
}