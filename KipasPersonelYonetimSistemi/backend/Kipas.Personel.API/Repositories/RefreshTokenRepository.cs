using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Data;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Repositories
{
    public class RefreshTokenRepository
        : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByHashAsync(
            string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(token => token.AppUser)
                .FirstOrDefaultAsync(token =>
                    token.TokenHash == tokenHash);
        }

        public async Task AddAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens
                .AddAsync(refreshToken);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}