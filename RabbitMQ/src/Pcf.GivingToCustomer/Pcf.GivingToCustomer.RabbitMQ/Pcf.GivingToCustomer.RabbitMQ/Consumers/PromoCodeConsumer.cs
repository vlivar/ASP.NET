using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.RabbitMQ.Consumers;

public class PromoCodeConsumer : IConsumer<PromoCodeRabbitDto>
{
    private readonly IRepository<Customer> _customersRepository;
    private readonly IRepository<PromoCode> _promoCodesRepository;
    private readonly IRepository<Preference> _preferencesRepository;


    public PromoCodeConsumer(IRepository<Customer> customersRepository,
                            IRepository<PromoCode> promoCodesRepository,
                            IRepository<Preference> preferencesRepository)
    {
        _customersRepository = customersRepository;
        _promoCodesRepository = promoCodesRepository;
        _preferencesRepository = preferencesRepository;
    }

    public async Task Consume(ConsumeContext<PromoCodeRabbitDto> context)
    {
        try
        {
            var promoCodeDto = context.Message;

            if (promoCodeDto != null)
            {
                Console.WriteLine($"Received PromoCode: {promoCodeDto.Code}, PartnerId: {promoCodeDto.PartnerId}");

                var preference = await _preferencesRepository.GetByIdAsync(promoCodeDto.PreferenceId);
                if (preference == null)
                    return;

                var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

                var promoCode = PromoCodeRabbitDtoMapper.MapFromModel(promoCodeDto, preference, customers);
                await _promoCodesRepository.AddAsync(promoCode);

                Console.WriteLine($"Handle promocode success!");
            }
            else
            {
                Console.WriteLine("Received PromoCode = null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }
}