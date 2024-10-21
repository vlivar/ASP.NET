using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Context;

namespace PromoCodeFactory.DataAccess.Repositories;
public class RoleRepository : EfRepository<Role>, IRoleRepository
{
    public RoleRepository(DataContext context) : base(context)
    {
    }
}
