namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public string PartnerName { get; set; }

        public string PromoCode { get; set; }

        public string Preference { get; set; }
    }
}