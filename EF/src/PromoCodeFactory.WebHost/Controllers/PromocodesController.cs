using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(IPromoCodeRepository _promoCodeRepository,
        IPreferenceRepository _preferenceRepository,
        ICustomerRepository _customerRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken cancellationToken)
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync(cancellationToken);

            var promoCodesResponse = promoCodes.Select(x =>
                new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate.ToString(),
                    EndDate = x.EndDate.ToString(),
                    PartnerName = x.PartnerName
                }).ToList();

            return promoCodesResponse;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<bool>> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken cancellationToken)
        {
            var allPreferences = await _preferenceRepository.GetAllAsync(cancellationToken);
            var preference = allPreferences.FirstOrDefault(p => p.Name == request.Preference);
            if (preference is null)
                return BadRequest($"Предпочтение {request.Preference} не найдено.");

            var customers = await _customerRepository.GetAllAsync(cancellationToken);
            if (customers is null)
                return false;

            foreach (var customer in customers)
            {
                PromoCode promoCode = new()
                {
                    Id = Guid.NewGuid(),
                    Code = $"{request.PromoCode}|{customer.Id}",
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(15),
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    PreferenceId = preference.Id,
                    CustomerId = customer.Id,
                    EmployeeId = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f")
                };

                await _promoCodeRepository.AddAsync(promoCode, cancellationToken);
                await _promoCodeRepository.SaveChangesAsync();

                customer.PromoCodes.Add(promoCode);
                await _customerRepository.UpdateAsync(customer, cancellationToken);
                await _customerRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}