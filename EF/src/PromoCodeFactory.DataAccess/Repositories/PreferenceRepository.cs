using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Context;

namespace PromoCodeFactory.DataAccess.Repositories;
public class PreferenceRepository : EfRepository<Preference>, IPreferenceRepository
{
    public PreferenceRepository(DataContext context) : base(context)
    {
    }
}
