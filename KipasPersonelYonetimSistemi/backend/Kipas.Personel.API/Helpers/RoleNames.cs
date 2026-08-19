namespace Kipas.Personel.API.Helpers
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string HumanResources = "HumanResources";
        public const string Manager = "Manager";
        public const string Employee = "Employee";

        public const string AdminOrHumanResources =
         Admin + "," + HumanResources;

        public static bool TryNormalize(
            string? role,
            out string normalizedRole)
        {
            normalizedRole = string.Empty;

            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            normalizedRole =
                role.Trim().ToLowerInvariant() switch
                {
                    "admin" => Admin,
                    "humanresources" => HumanResources,
                    "manager" => Manager,
                    "employee" => Employee,
                    _ => string.Empty
                };

            return normalizedRole.Length > 0;
        }
    }
}