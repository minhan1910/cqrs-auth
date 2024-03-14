using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authorisation
{
    public static class AppFeature
    {
        public const string Employees = nameof(Employees);
        public const string Users = nameof(Users);
        public const string Roles = nameof(Roles);
        public const string RoleClaims = nameof(RoleClaims);
        public const string UserRoles = nameof(UserRoles);
        public const string UserClaims = nameof(UserClaims);
    }
}
