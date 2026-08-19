namespace Kipas.Personel.API.Helpers
{
    public enum UserManagementOperationStatus
    {
        Success,
        UserNotFound,
        UsernameAlreadyExists,
        EmployeeNotFound,
        EmployeeInactive,
        EmployeeAlreadyLinked,
        InvalidRole,
        CannotDeactivateSelf
    }
}