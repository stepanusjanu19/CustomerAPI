using System;
using Domain.Model;

namespace Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByCustomerCodeExceptIdAsync(string customerCode, Guid id);
        Task<Customer?> GetByCustomerCodeAsync(string customerCode);
        Task<bool> GetByCustomerCodeExist(string customerCode, Guid? id = null);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(Guid id);
    }
}