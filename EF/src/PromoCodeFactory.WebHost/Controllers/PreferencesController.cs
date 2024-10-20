using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.WebHost.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController(IPreferenceRepository _preferenceRepository) : ControllerBase
{
    /// <summary>
    /// Получение списка всех возможных предпочтений
    /// </summary>
    /// <returns>Список предпочтений</returns>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<PreferenceResponse>>> GetAllPreferencesAsync(CancellationToken cancellationToken)
    {
        var preferences = await _preferenceRepository.GetAllAsync(cancellationToken);
        var preferencesResponse = preferences.Select(p => new PreferenceResponse(p.Id, p.Name)).ToList();

        return preferencesResponse;
    }
}
