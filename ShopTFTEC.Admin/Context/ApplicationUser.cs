using Microsoft.AspNetCore.Identity;

namespace ShopTFTEC.Admin.Context;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }  = string.Empty;
    public string LastName { get; set; } = String.Empty;   
}
