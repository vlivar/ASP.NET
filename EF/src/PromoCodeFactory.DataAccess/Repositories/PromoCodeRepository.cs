using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Context;

namespace PromoCodeFactory.DataAccess.Repositories;
public class PromoCodeRepository : EfRepository<PromoCode>, IPromoCodeRepository
{
    public PromoCodeRepository(DataContext context) : base(context)
    {
    }
}
