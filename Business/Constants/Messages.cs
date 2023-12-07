namespace Business.Constants;

public static class Messages
{
    public static class Role
    {
        public static string Listed = "Roles successfully listed.";
        public static string AlreadyExists = "Another role with the same name already exists.";
        public static string Created = "Role created successfully.";
        public static string FailedCreate = "Failed to create role.";
        public static string FailedUpdate = "Failed to update role.";
        public static string NotFound = "Role not found.";
        public static string UpdatedSuccessfully = "Role updated successfully.";
        public static string NotUpdateSuperAdmin = "Role Super-Admin not update.";
    }
    public static class Claims
    {
        public static string Listed = "Claims successfully listed.";
        public static string Added = "Role claim added successfully.";
        public static string FailedAdded = "Role claim added failed.";
        public static string FailedRemoved = "Failed to calim.";
        public static string FailedUpdate = "Failed to update role.";
        public static string Updated = "Role claims update successfuly.";
    }
}
