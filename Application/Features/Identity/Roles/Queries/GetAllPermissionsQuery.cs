using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Roles.Queries
{
    public class GetAllPermissionsQuery : IRequest<IResponseWrapper>
    {
        public string RoleId { get; set; }
    }

    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, IResponseWrapper>
    {
        private readonly IRoleService _roleService;

        public GetAllPermissionsQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IResponseWrapper> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _roleService.GetPermissionsAsync(request.RoleId);
        }
    }
}
