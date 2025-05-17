using System;
using System.Collections.Generic;
using AutoMapper;
using UnitTests.Mocks;
using UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Xunit;
using Moq;
using Application.DTO;
using Application.Interfaces;
using Application.Request;
using Application.Extensions;
using Application.Response;
using Domain.Model;

namespace UnitTests.Controllers
{
    public class CustomerControllerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ICustomerRepository> _customerRepository;
        private readonly CustomerController _customerController;

        public CustomerControllerTests()
        {
            this._mapper = AutoMapperConfig.GetMapper();
            this._customerRepository = MockCustomerRepository.GetCustomerRepository();
            this._customerController = new CustomerController(this._customerRepository.Object, this._mapper, new CustomExtensions());
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCustomersExist()
        {
            var result = await this._customerController.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);

            var response = Assert.IsType<ApiResponse<IEnumerable<Customer>>>(okResult.Value);
            Assert.Equal("Success", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCustomerExists()
        {
            var customer = (await this._customerRepository.Object.GetAllAsync()).First();
            var result = await this._customerController.GetById(customer.CustomerId);
            var okResult = Assert.IsType<OkObjectResult>(result);

            var response = Assert.IsType<ApiResponse<Customer>>(okResult.Value);
            Assert.Equal("Success", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
            Assert.NotNull(response.Data);
            Assert.Equal(customer.CustomerId, response.Data.CustomerId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            var result = await this._customerController.GetById(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Customer nof found", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenCustomerIsValid()
        {
            var request = new CreateCustomerRequest
            {
                CustomerName = "Adi Suhanto",
                CustomerAddress = "123 Roy St",
                CreatedBy = 0
            };

            var result = await this._customerController.Create(request);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            var response = Assert.IsType<ApiResponse<CustomerDto>>(createdResult.Value);
            Assert.Equal("Customer created", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
            Assert.NotNull(response.Data);
            Assert.Equal(request.CustomerName, response.Data.CustomerName);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenValidUpdate()
        {
            var customer = (await this._customerRepository.Object.GetAllAsync()).First();

            var request = new UpdateCustomerRequest
            {
                CustomerName = "Adi Suhanto E",
                CustomerAddress = "123 Sun St",
                ModifiedBy = 0
            };

            var result = await this._customerController.Update(customer.CustomerId, request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            var response = Assert.IsType<ApiResponse<CustomerDto>>(okResult.Value);
            Assert.Equal("Customer updated", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
            Assert.NotNull(response.Data);
            Assert.Equal(request.CustomerName, response.Data.CustomerName);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenCustomerExists()
        {
            var customer = (await this._customerRepository.Object.GetAllAsync()).First();
            var result = await this._customerController.Delete(customer.CustomerId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Customer deleted", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            var result = await this._customerController.Delete(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Customer not found", response.Message);
            Assert.NotEqual(Guid.Empty, response.TransactionId);
        }

    }
}
