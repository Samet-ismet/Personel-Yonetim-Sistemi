using Microsoft.EntityFrameworkCore;
using Kipas.Personel.API.Data;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;
using Kipas.Personel.API.Interfaces;

namespace Kipas.Personel.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(
            List<Employee> Items,
            int TotalCount)> GetPagedAsync(
                EmployeeQueryParameters queryParameters)
        {
            IQueryable<Employee> query =
                _context.Employees
                    .AsNoTracking()
                    .Include(employee =>
                        employee.Department);

            if (!string.IsNullOrWhiteSpace(
                    queryParameters.Search))
            {
                var search =
                    queryParameters.Search.Trim();

                query = query.Where(employee =>
                    employee.FirstName.Contains(search) ||
                    employee.LastName.Contains(search));
            }

            if (queryParameters.DepartmentId.HasValue)
            {
                var departmentId =
                    queryParameters.DepartmentId.Value;

                query = query.Where(employee =>
                    employee.DepartmentId ==
                    departmentId);
            }

            var totalCount =
                await query.CountAsync();

            var sortBy =
                queryParameters.SortBy?
                    .Trim()
                    .ToLowerInvariant()
                ?? "id";

            var descending =
                string.Equals(
                    queryParameters.SortDirection?
                        .Trim(),
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            query = (sortBy, descending) switch
            {
                ("firstname", false) =>
                    query.OrderBy(employee =>
                        employee.FirstName),

                ("firstname", true) =>
                    query.OrderByDescending(employee =>
                        employee.FirstName),

                ("lastname", false) =>
                    query.OrderBy(employee =>
                        employee.LastName),

                ("lastname", true) =>
                    query.OrderByDescending(employee =>
                        employee.LastName),

                ("department", false) =>
                    query.OrderBy(employee =>
                        employee.Department.Name),

                ("department", true) =>
                    query.OrderByDescending(employee =>
                        employee.Department.Name),

                ("id", true) =>
                    query.OrderByDescending(employee =>
                        employee.Id),

                _ =>
                    query.OrderBy(employee =>
                        employee.Id)
            };

            var employees =
                await query
                    .Skip(
                        (queryParameters.PageNumber - 1) *
                        queryParameters.PageSize)
                    .Take(
                        queryParameters.PageSize)
                    .ToListAsync();

            return (
                employees,
                totalCount);
        }

        public async Task<Employee?> GetByIdAsync(
            int id)
        {
            return await _context.Employees
                .Include(employee =>
                    employee.Department)
                .FirstOrDefaultAsync(employee =>
                    employee.Id == id);
        }

        public async Task<bool> HasLinkedUserAsync(
            int employeeId)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(user =>
                    user.EmployeeId == employeeId);
        }

        public async Task AddAsync(
            Employee employee)
        {
            await _context.Employees
                .AddAsync(
                    employee);
        }

        public Task UpdateAsync(
            Employee employee)
        {
            // Employee GetByIdAsync ile
            // getirildiği için DbContext
            // tarafından zaten takip edilir.
            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            Employee employee)
        {
            _context.Employees.Remove(
                employee);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}