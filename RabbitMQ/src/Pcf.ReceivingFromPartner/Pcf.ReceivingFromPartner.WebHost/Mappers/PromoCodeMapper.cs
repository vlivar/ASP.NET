using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.RabbitMQ;
using Pcf.ReceivingFromPartner.WebHost.Models;

namespace Pcf.ReceivingFromPartner.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(ReceivingPromoCodeRequest request, Preference preference, Partner partner)
        {

            var promocode = new PromoCode();

            promocode.PartnerId = partner.Id;
            promocode.Partner = partner;
            promocode.Code = request.PromoCode;
            promocode.ServiceInfo = request.ServiceInfo;

            promocode.BeginDate = DateTime.Now;
            promocode.EndDate = DateTime.Now.AddDays(30);

            promocode.Preference = preference;
            promocode.PreferenceId = preference.Id;

            promocode.PartnerManagerId = request.PartnerManagerId;

            return promocode;
        }

        public static PromoCodeRabbitDto MapFromRabbitDto(PromoCode promoCode)
        {
            var promoCodeRabbitDto = new PromoCodeRabbitDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PartnerId = promoCode.PartnerId,
                PartnerManagerId = promoCode.PartnerManagerId,
                PreferenceId = promoCode.PreferenceId
            };

            return promoCodeRabbitDto;
        }
    }
}
