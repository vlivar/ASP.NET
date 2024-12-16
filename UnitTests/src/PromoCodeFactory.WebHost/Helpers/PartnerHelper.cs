using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;

namespace PromoCodeFactory.WebHost.Helpers
{
    public static class PartnerHelper
    {
        public static void ProcessActiveLimit(this Partner partner)
        {
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue);
            if (activeLimit != null)
            {
                partner.NumberIssuedPromoCodes = 0;
                activeLimit.CancelDate = DateTime.Now;
            }
        }

        public static PartnerPromoCodeLimit CreateNewLimit(Partner partner, SetPartnerPromoCodeLimitRequest request)
        {
            return new PartnerPromoCodeLimit
            {
                Limit = request.Limit,
                Partner = partner,
                PartnerId = partner.Id,
                CreateDate = DateTime.Now,
                EndDate = request.EndDate
            };
        }
    }
}
