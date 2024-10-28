using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models.Requests;

public class RoleRequest
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }
}
