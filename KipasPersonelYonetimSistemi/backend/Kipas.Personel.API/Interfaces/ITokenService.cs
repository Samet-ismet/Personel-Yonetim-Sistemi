using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(
            AppUser user,
            out DateTime expiration);

        string CreateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}