using System;
using Application.Interfaces;
using Application.Extensions;
using Domain.Model;
using Moq;
using System.Collections.Generic;

namespace UnitTests.Mocks
{
    public static class MockCustomerRepository
    {
        public static Mock<ICustomerRepository> GetCustomerRepository()
        {
            var mockRepository = new Mock<ICustomerRepository>();

            var customExtensions = new CustomExtensions();

            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Adi Santoso",
                    CustomerAddress = "123 Yuki St",
                    CustomerCode = customExtensions.GenerateUniqueCustomerCode(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0
                }
            };

            mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(customers);

            mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => customers.Find(x => x.CustomerId == id));

            mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) =>
                {
                    customers.Add(customer);
                    return customer;
                });

            mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) => customer);

            mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == id);
                    if (customer != null)
                    {
                        customers.Remove(customer);
                        return true;
                    }
                    return false;
                });


            return mockRepository;
        }
    }
}

