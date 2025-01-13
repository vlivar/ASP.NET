using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Cache;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;

        private readonly PreferenceCache _preferencesCache;

        public PromocodesController(
            IRepository<PromoCode> promoCodesRepository, 
            IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository,
            PreferenceCache preferencesCache)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
            _preferencesCache = preferencesCache;
        }
        
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await _promoCodesRepository.GetAllAsync();

            var response = promocodes.Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = x.PartnerId,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            try
            {
                Preference? preference = null;
                preference = await _preferencesCache.GetPreferenceAsync(request.PreferenceId);

                if (preference == null)
                {
                    preference = await _preferencesRepository.GetByIdAsync(request.PreferenceId);
                    if (preference != null)
                    {
                        Console.WriteLine($"Preference извлечен из базы данных");
                        await _preferencesCache.SavePreferenceAsync(preference);
                    }
                    else
                    {
                        return BadRequest("Предпочтение не найдено");
                    }
                }
                else
                {
                    Console.WriteLine($"Preference извлечен из cache");
                }

                var customers = await _customersRepository
                    .GetWhere(d => d.Preferences.Any(x =>
                        x.Preference.Id == preference.Id));

                PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, customers);

                await _promoCodesRepository.AddAsync(promoCode);

                return CreatedAtAction(nameof(GetPromocodesAsync), new { }, null);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}