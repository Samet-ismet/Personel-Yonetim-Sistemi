using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash);

        Task AddAsync(RefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}