using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Context;

namespace PromoCodeFactory.DataAccess.Repositories;
public class CustomerRepository : EfRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(DataContext context) : base(context)
    {
    }
}
