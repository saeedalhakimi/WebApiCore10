namespace WebApiCore10.RustApi.Presentation.Routing
{
    public static class ApiRoutes
    {
        public static class AuthRoutes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/auth";
            public const string Login = "login";
            public const string Register = "register";
            public const string Logout = "logout";
            public const string RefreshToken = "refresh-token";
        }

        public static class RoleRoutes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/roles";
            public const string CreateRole = "create";
            //public const string GetAllRoles = "all";
            public const string GetRoleById = "{id}";
            public const string UpdateRole = "update/{id}";
            public const string DeleteRole = "delete/{id}";
        }

        public static class UserRoutes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/users";
            public const string GetAllUsers = "all";
            public const string GetUserById = "{id}";
            public const string UpdateUser = "update/{id}";
            public const string DeleteUser = "delete/{id}";
        }

        public static class UserProfileRoutes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/userprofiles";
            public const string GetAllUserProfiles = "all";
            public const string GetUserProfileById = "{id}";
            public const string UpdateUserProfile = "update/{id}";
            public const string DeleteUserProfile = "delete/{id}";
        }
    }
}
