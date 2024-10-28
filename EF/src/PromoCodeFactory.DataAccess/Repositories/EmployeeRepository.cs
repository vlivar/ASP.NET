using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Context;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EmployeeRepository : EfRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(DataContext context) : base(context)
    {
    }
}
