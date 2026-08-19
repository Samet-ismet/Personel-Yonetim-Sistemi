using Kipas.Personel.API.DTOs;

namespace Kipas.Personel.API.Helpers
{
    public sealed class UserManagementOperationResult
    {
        public UserManagementOperationStatus Status
        {
            get;
            init;
        }

        public UserDto? User
        {
            get;
            init;
        }
    }
}