using System;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class GivePromoCodeRequest
    {
        [Required]
        public string ServiceInfo { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public string PromoCode { get; set; }

        [Required]
        public string Preference { get; set; }
    }
}