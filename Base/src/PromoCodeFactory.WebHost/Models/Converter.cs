using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models.Responses;

namespace PromoCodeFactory.WebHost.Models
{
    internal static class Converter
    {
        internal static Role ConverterRole(RoleItemResponse roleItem)
        {
            return new Role
            {
                Description = roleItem.Description,
                Name = roleItem.Name,
                Id = roleItem.Id
            };
        }
    }
}
