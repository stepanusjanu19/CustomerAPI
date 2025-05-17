using System.Data;
using System.Collections.Generic;
using System.Net.Mime;
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces;
using Application.Exceptions;
using Application.Extensions;
using Application.Request;
using Application.Response;
using AutoMapper;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly CustomExtensions _customExtensions;
        public CustomerController(ICustomerRepository customerRepository, IMapper mapper, CustomExtensions customExtensions)
        {
            this._customerRepository = customerRepository;
            this._mapper = mapper;
            this._customExtensions = customExtensions;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await this._customerRepository.GetAllAsync();
            return (data == null || !data.Any())
                ? NotFound(new ApiResponse<List<object>>("Customer data nof found", new List<object>()))
                : Ok(new ApiResponse<IEnumerable<Customer>>("Success", data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await this._customerRepository.GetByIdAsync(id);
            return data == null
                ? NotFound(new ApiResponse<object>("Customer nof found", new object{}))
                : Ok(new ApiResponse<Customer>("Success", data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(
                    "Validation Failed",
                    ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                ));

            try
            {
                var customerEntity = this._mapper.Map<Customer>(request);
                customerEntity.CustomerId = Guid.NewGuid();
                customerEntity.CreatedAt = DateTime.UtcNow;

                if (string.IsNullOrEmpty(request.CustomerCode))
                {
                    string generateCode;
                    int attempt = 0;

                    do
                    {
                        generateCode = this._customExtensions.GenerateUniqueCustomerCode();
                        attempt++;

                        if (attempt > 10)
                            return StatusCode(500, new ApiResponse<object>("Failed to generate unique CustomerCode."));


                    } while (await this._customerRepository.GetByCustomerCodeExist(generateCode));

                    customerEntity.CustomerCode = generateCode;
                }
                else
                {
                    var existingData = await this._customerRepository.GetByCustomerCodeAsync(request.CustomerCode);

                    if (existingData != null)
                    {
                        return Conflict(
                            new ApiResponse<object>(new DuplicateCustomerException(existingData.CustomerCode, existingData.CustomerName).Message));
                    }

                    customerEntity.CustomerCode = request.CustomerCode?.Trim();
                }

                var customerResult = await this._customerRepository.AddAsync(customerEntity);
                var customerMap = this._mapper.Map<CustomerDto>(customerResult);

                return CreatedAtAction(nameof(GetById), new { id = customerMap.CustomerId },
                    new ApiResponse<CustomerDto>("Customer created", customerMap));
            } catch (DuplicateCustomerException ex) {
                return Conflict(new ApiResponse<object>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(
                    "Validation Failed",
                    ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                ));

            var existingData = await this._customerRepository.GetByIdAsync(id);
            if (existingData == null)
                return NotFound(new ApiResponse<object>("Customer not found", new object{}));

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                existingData.CustomerName = request.CustomerName?.Trim();

            if (!string.IsNullOrWhiteSpace(request.CustomerAddress))
                existingData.CustomerAddress = request.CustomerAddress?.Trim();

            if (!string.Equals(existingData.CustomerCode, request.CustomerCode, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(request.CustomerCode))
                {
                    var conflictData = await this._customerRepository.GetByCustomerCodeExceptIdAsync(request.CustomerCode, id);

                    if (conflictData != null)
                    {
                        return Conflict(new ApiResponse<object>(
                            new DuplicateCustomerException(conflictData.CustomerCode, conflictData.CustomerName).Message));
                    }

                    existingData.CustomerCode = request.CustomerCode?.Trim();
                }
            }

            this._mapper.Map(request, existingData);

            if (request.ModifiedBy.HasValue)
                existingData.ModifiedBy = request.ModifiedBy.Value;

            existingData.ModifiedAt = DateTime.UtcNow;

            var customerResult = await this._customerRepository.UpdateAsync(existingData);
            var customerMap = this._mapper.Map<CustomerDto>(customerResult);

            return Ok(new ApiResponse<CustomerDto>("Customer updated", customerMap));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingData = await this._customerRepository.GetByIdAsync(id);
            if (existingData == null)
            {
                return NotFound(new ApiResponse<object>("Customer not found", new object{}));
            }

            await this._customerRepository.DeleteAsync(id);

            return Ok(new ApiResponse<object>("Customer deleted"));
        }
    }
}
