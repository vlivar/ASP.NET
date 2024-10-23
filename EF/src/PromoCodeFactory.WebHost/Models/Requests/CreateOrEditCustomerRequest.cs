using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class CreateOrEditCustomerRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        public string Email { get; set; }

        public List<Guid> PreferenceIds { get; set; }
    }
}