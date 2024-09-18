using PromoCodeFactory.WebHost.Models.Responses;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models.Requests;

public class EmployeeRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public List<RoleItemResponse> Roles { get; set; }
}