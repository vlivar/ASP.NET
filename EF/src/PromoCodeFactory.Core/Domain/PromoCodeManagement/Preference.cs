using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement;

public class Preference : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public virtual List<CustomerPreference> CustomerPreferences { get; set; }
}