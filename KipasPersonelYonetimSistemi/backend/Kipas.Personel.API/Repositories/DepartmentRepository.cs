using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Data;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Repositories
{
    public class DepartmentRepository
        : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync(
            bool includeInactive)
        {
            IQueryable<Department> query =
                _context.Departments.AsNoTracking();

            if (!includeInactive)
            {
                query = query.Where(
                    department => department.IsActive);
            }

            return await query
                .OrderBy(department => department.Name)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(
            int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(
                    department => department.Id == id);
        }

        public async Task<bool> NameExistsAsync(
            string name,
            int? excludedDepartmentId = null)
        {
            return await _context.Departments
                .AnyAsync(department =>
                    department.Name == name &&
                    (!excludedDepartmentId.HasValue ||
                     department.Id !=
                     excludedDepartmentId.Value));
        }

        public async Task AddAsync(
            Department department)
        {
            await _context.Departments.AddAsync(
                department);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}