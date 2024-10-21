using System;

namespace PromoCodeFactory.WebHost.Models.Response;

public record class PreferenceResponse(Guid Id, string Name, string Description);