using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;

namespace Pcf.GivingToCustomer.RabbitMQ;

public class PromoCodeRabbitDtoMapper
{
    public static PromoCode MapFromModel(PromoCodeRabbitDto dto, Preference preference, IEnumerable<Customer> customers)
    {

        var promocode = new PromoCode();
        promocode.Id = dto.Id;

        promocode.PartnerId = dto.PartnerId;
        promocode.Code = dto.Code;
        promocode.ServiceInfo = dto.ServiceInfo;

        promocode.BeginDate = DateTime.Parse(dto.BeginDate);
        promocode.EndDate = DateTime.Parse(dto.EndDate);

        promocode.Preference = preference;
        promocode.PreferenceId = preference.Id;

        promocode.Customers = new List<PromoCodeCustomer>();

        foreach (var item in customers)
        {
            promocode.Customers.Add(new PromoCodeCustomer()
            {

                CustomerId = item.Id,
                Customer = item,
                PromoCodeId = promocode.Id,
                PromoCode = promocode
            });
        };

        return promocode;
    }
}
