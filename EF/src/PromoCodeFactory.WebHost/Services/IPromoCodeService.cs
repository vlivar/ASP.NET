using PromoCodeFactory.WebHost.Models.Requests;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models.Response;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Services;

public interface IPromoCodeService
{
    Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken cancellationToken);
    Task<List<PromoCodeShortResponse>> GetAllPromoCodesAsync(CancellationToken cancellationToken);
}