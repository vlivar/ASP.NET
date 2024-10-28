using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(ICustomerRepository _customerRepository,
        IPreferenceRepository _preferenceRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получение списка всех клиентов
        /// </summary>
        [HttpGet("all")]
        public async Task<List<CustomerShortResponse>> GetAllCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            var customersResponse = customers.Select(x =>
                new CustomerShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToList();

            return customersResponse;
        }

        /// <summary>
        /// Получение клиента по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetAsync(id, cancellationToken);
            if (customer == null)
            {
                return NotFound();
            }

            List<PreferenceResponse> preferences = new();
            foreach (var customerPreference in customer.CustomerPreferences)
            {
                var pref = await _preferenceRepository.GetAsync(customerPreference.PreferenceId, cancellationToken);
                if (pref != null)
                {
                    preferences.Add(new PreferenceResponse(pref.Id, pref.Name, pref.Description ?? ""));
                }
            }

            List<PromoCodeShortResponse> promoCodes = new();
            foreach (var customerPromo in customer.PromoCodes)
            {
                promoCodes.Add(new PromoCodeShortResponse
                {
                    Id = customerPromo.Id,
                    Code = customerPromo.Code,
                    ServiceInfo = customerPromo.ServiceInfo,
                    BeginDate = customerPromo.BeginDate.ToString(),
                    EndDate = customerPromo.EndDate.ToString(),
                    PartnerName = customerPromo.PartnerName
                });
            }

            var customerResponse = new CustomerResponse
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Preferences = preferences,
                PromoCodes = promoCodes
            };

            return customerResponse;
        }

        /// <summary>
        /// Создание нового клиента
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<bool>> CreateCustomerAsync(CreateOrEditCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                };

                customer.CustomerPreferences = request.PreferenceIds.Select(pid => new CustomerPreference
                {
                    CustomerId = customer.Id,
                    PreferenceId = pid
                }).ToList();

                var newCustomer = await _customerRepository.AddAsync(customer, cancellationToken);
                if (newCustomer == null)
                {
                    return BadRequest();
                }

                return true;
            }
            finally
            {
                await _customerRepository.SaveChangesAsync();
            }
        }

        [HttpPut("edit{id}")]
        public async Task<ActionResult<bool>> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var editCustomer = await _customerRepository.GetAsync(id, cancellationToken);
                if (editCustomer == null)
                {
                    return NotFound();
                }

                editCustomer.CustomerPreferences.Clear();

                editCustomer.FirstName = request.FirstName;
                editCustomer.LastName = request.LastName;
                editCustomer.Email = request.Email;
                editCustomer.CustomerPreferences = request.PreferenceIds.Select(pid => new CustomerPreference
                {
                    CustomerId = editCustomer.Id,
                    PreferenceId = pid
                }).ToList();

                var updateCustomer = await _customerRepository.UpdateAsync(editCustomer, cancellationToken);
                if (updateCustomer == null)
                {
                    return BadRequest();
                }

                return true;
            }
            finally
            {
                await _customerRepository.SaveChangesAsync();
            }
        }

        [HttpDelete("delete{id}")]
        public async Task<ActionResult<bool>> DeleteCustomer(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(id, cancellationToken);
                if (customer == null)
                {
                    return NotFound();
                }

                return await _customerRepository.DeleteAsync(id, cancellationToken);
            }
            finally
            {
                await _customerRepository.SaveChangesAsync();
            }
        }
    }
}