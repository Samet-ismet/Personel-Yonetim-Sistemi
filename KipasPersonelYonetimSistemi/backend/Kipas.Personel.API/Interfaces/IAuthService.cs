using Kipas.Personel.API.DTOs;

namespace Kipas.Personel.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(
            LoginDto dto);

        Task<AuthResponseDto?> RefreshAsync(
            string refreshToken);

        Task LogoutAsync(
            string refreshToken);
    }
}