using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Data;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Repositories
{
    public class AppUserRepository
        : IAppUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AppUserRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppUser>>
            GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(user => user.Employee)
                    .ThenInclude(employee =>
                        employee!.Department)
                .OrderBy(user => user.Username)
                .ToListAsync();
        }

        public async Task<AppUser?> GetByIdAsync(
            int id)
        {
            return await _context.Users
                .Include(user => user.Employee)
                    .ThenInclude(employee =>
                        employee!.Department)
                .FirstOrDefaultAsync(
                    user => user.Id == id);
        }

        public async Task<AppUser?>
            GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.Username == username);
        }

        public async Task<bool>
            UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(
                    user =>
                        user.Username == username);
        }

        public async Task<bool>
            EmployeeIsLinkedAsync(
                int employeeId,
                int? excludedUserId = null)
        {
            return await _context.Users
                .AnyAsync(user =>
                    user.EmployeeId == employeeId &&
                    (!excludedUserId.HasValue ||
                     user.Id != excludedUserId.Value));
        }

        public async Task AddAsync(AppUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}