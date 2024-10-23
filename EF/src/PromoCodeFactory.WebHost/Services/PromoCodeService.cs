using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Requests;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using PromoCodeFactory.WebHost.Models.Response;

namespace PromoCodeFactory.WebHost.Services;


public class PromoCodeService : IPromoCodeService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPromoCodeRepository _promoCodeRepository;

    public PromoCodeService(
        IPreferenceRepository preferenceRepository,
        IEmployeeRepository employeeRepository,
        ICustomerRepository customerRepository,
        IPromoCodeRepository promoCodeRepository)
    {
        _preferenceRepository = preferenceRepository;
        _employeeRepository = employeeRepository;
        _customerRepository = customerRepository;
        _promoCodeRepository = promoCodeRepository;
    }

    public async Task<List<PromoCodeShortResponse>> GetAllPromoCodesAsync(CancellationToken cancellationToken)
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

    public async Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var allPreferences = await _preferenceRepository.GetAllAsync(cancellationToken);
        var preference = allPreferences.FirstOrDefault(p => p.Name == request.Preference);
        if (preference is null)
            throw new ArgumentException($"Предпочтение {request.Preference} не найдено.");

        var partner = await _employeeRepository.GetAsync(request.EmployeeId, cancellationToken);
        if (partner is null)
            throw new ArgumentException($"Партнер с EmployeeId={request.EmployeeId} не найден.");

        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        if (!customers.Any())
            return false;

        foreach (var customer in customers)
        {
            var promoCode = new PromoCode()
            {
                Id = Guid.NewGuid(),
                Code = $"{request.PromoCode}|{customer.Id}",
                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15),
                ServiceInfo = request.ServiceInfo,
                PartnerName = partner.FullName,
                PreferenceId = preference.Id,
                CustomerId = customer.Id,
                EmployeeId = request.EmployeeId
            };

            await _promoCodeRepository.AddAsync(promoCode, cancellationToken);
            customer.PromoCodes.Add(promoCode);
            await _customerRepository.UpdateAsync(customer, cancellationToken);

            await _customerRepository.SaveChangesAsync();
        }

        return true;
    }
}