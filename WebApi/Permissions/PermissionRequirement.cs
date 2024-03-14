﻿using Microsoft.AspNetCore.Authorization;

namespace WebApi.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; set; }

    }
}
