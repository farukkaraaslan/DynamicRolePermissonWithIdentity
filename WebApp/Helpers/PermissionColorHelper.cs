namespace WebApp.Helpers
{
    public static class PermissionColorHelper
    {
        public static string GetPermissionColor(string permission)
        {
            string[] parts = permission.Split('.');
            string action = parts.Length > 2 ? parts[2].ToLower() : "";

            switch (action)
            {
                case "create":
                    return "badge-info";
                case "read":
                    return "badge-success";
                case "update":
                    return "badge-warning";
                case "delete":
                    return "badge-danger";
                default:
                    return "badge-secondary";
            }
        }
    }

}
