using Microsoft.AspNetCore.Identity;
using ShopTFTEC.Admin.Context;

namespace ShopTFTEC.Admin.Areas.Admin.Models;

public class RoleEdit
{
    public IdentityRole? Role { get; set; }
    public IEnumerable<ApplicationUser>? Members { get; set; }
    public IEnumerable<ApplicationUser>? NonMembers { get; set; }
}
