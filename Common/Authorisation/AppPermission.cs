using System.Collections.ObjectModel;

namespace Common.Authorisation
{
    public record AppPermission(string Feature,
                                string Action,
                                string Group,
                                string Description,
                                bool IsBasic = false)
    {
        public string Name => NameFor(Feature, Action);
        public static string NameFor(string feature, string action)
            => $"Permissions.{feature}.{action}";
    }

    // Can we use Abstract factory for creating AppPermission each feature
    public class AppPermissions                             
    {
        /**
         * Think about how can I custom Description -> make this method more flexible for creating AppPermission list
         * or using Abstract factory
         */
        //public static List<AppPermission> GenerateListAppPermissionFor(string feature, 
        //                                                        string roleGroup,                                      
        //                                                        IReadOnlyList<string> actions = null)
        //{
        //    if (actions is not null || actions.Any())
        //    {
        //        actions.ToList().AddRange(AppAction.FullActions);
        //    }

        //    var appPermisisons = new List<AppPermission>();

        //    foreach (var action in actions) 
        //    {
        //        appPermisisons.Add(
        //            new(feature, action, roleGroup, $"{action} {feature}"));
        //    }

        //    return appPermisisons;
        //}

        //private static AppPermission[]  GetAppPermissions()
        //{
        //    var appPermissions = new List<AppPermission>();

        //    var users = GenerateListAppPermissionFor(AppFeature.Users, AppRoleGroup.SystemAccess, AppAction.FullActions);

        //    var userRoles = GenerateListAppPermissionFor(AppFeature.UserRoles, AppRoleGroup.SystemAccess, AppAction.FullActions);
        //}


        private static readonly AppPermission[] _all = new AppPermission[]
            {
                new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
                new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
                new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
                new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

                new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles"),
                new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

                new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
                new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles"),
                new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
                new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

                new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role Claims/Permissions"),
                new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),

                new(AppFeature.Employees, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Employees"),
                new(AppFeature.Employees, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Employees", IsBasic: true),
                new(AppFeature.Employees, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Employees"),
                new(AppFeature.Employees, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Employees"),
            };

        public static IReadOnlyList<AppPermission> AdminPermissions { get; }
            = new ReadOnlyCollection<AppPermission>(_all.Where(p => !p.IsBasic).ToArray());

        public static IReadOnlyList<AppPermission> BasicPermissions { get; }
            = new ReadOnlyCollection<AppPermission>(_all.Where(p => p.IsBasic).ToArray());

        public static IReadOnlyList<AppPermission> AllPermissions { get; }
            = new ReadOnlyCollection<AppPermission>(_all);
    }
}