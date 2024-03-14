﻿using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Commands
{
    public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
    {
        public UpdateUserRoleRequest UpdateUserRole { get; set; }
    }

    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
    {

        private readonly IUserService _userService;

        public UpdateUserRolesCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserRolesAsync(request.UpdateUserRole);
        }
    }
}
