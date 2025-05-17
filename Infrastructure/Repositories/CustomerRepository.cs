using System.Runtime.Intrinsics.X86;
using System;
using Application.Interfaces;
using Application.Exceptions;
using Domain.Model;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context) => _context = context;

        public async Task<Customer> AddAsync(Customer customer)
        {

            var existing = await this._context.Customers
                    .Where(c => c.CustomerCode == customer.CustomerCode)
                    .FirstOrDefaultAsync();

            if (existing != null)
                throw new DuplicateCustomerException(existing.CustomerCode, existing.CustomerName);

            customer.CustomerId = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;

            this._context.Customers.Add(customer);
            await this._context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existingData = await this._context.Customers.FindAsync(customer.CustomerId);
            if (existingData == null) return null;

            if (!string.Equals(existingData.CustomerCode, customer.CustomerCode, StringComparison.OrdinalIgnoreCase))
            {
                var conflictData = await this._context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerCode == customer.CustomerCode && c.CustomerId != customer.CustomerId);

                if (conflictData != null)
                    throw new DuplicateCustomerException(conflictData.CustomerCode, conflictData.CustomerName);
            }

            existingData.CustomerCode = customer.CustomerCode;
            existingData.CustomerName = customer.CustomerName;
            existingData.CustomerAddress = customer.CustomerAddress;
            existingData.ModifiedAt = DateTime.UtcNow;
            existingData.ModifiedBy = customer.ModifiedBy;

            await this._context.SaveChangesAsync();

            return existingData;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await this._context.Customers.FindAsync(id);
            if (entity == null) return false;
            this._context.Customers.Remove(entity);
            return await this._context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync() =>
            await this._context.Customers.ToListAsync();

        public async Task<Customer?> GetByIdAsync(Guid id) =>
            await this._context.Customers.FindAsync(id);

        public async Task<Customer?> GetByCustomerCodeExceptIdAsync(string customerCode, Guid id) =>
            await this._context.Customers
                .Where(c => (c.CustomerCode.ToLower().Contains(customerCode.ToLower()) || c.CustomerCode.ToUpper().Contains(customerCode.ToUpper())) && c.CustomerId != id)
                .FirstOrDefaultAsync();

        public async Task<Customer?> GetByCustomerCodeAsync(string customerCode) =>
            await this._context.Customers
                .Where(c => c.CustomerCode.ToLower().Contains(customerCode.ToLower()) || c.CustomerCode.ToUpper().Contains(customerCode.ToUpper()))
                .FirstOrDefaultAsync();

        public async Task<bool> GetByCustomerCodeExist(string customerCode, Guid? id = null) =>
            await this._context.Customers.AnyAsync(c =>
                c.CustomerCode == customerCode &&
                (!id.HasValue || c.CustomerId != id.Value));
        

    }
}