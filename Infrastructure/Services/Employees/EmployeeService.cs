﻿using Application.Services.Employees;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<int> DeleteEmployeeAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return employee.Id;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return _context.Employees.Where(emp => emp.Id == id).FirstOrDefault();
        }

        public async Task<List<Employee>> GetEmployeeListAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();

            return employee;
        }
    }
}