using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Models.Requests;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(IPromoCodeRepository promoCodeRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync(CancellationToken cancellationToken)
        {
            var promoCodes = await promoCodeRepository.GetAllAsync(cancellationToken);

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
        public Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
            throw new NotImplementedException();
        }
    }
}