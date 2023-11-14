using Microsoft.AspNetCore.Identity;
using ShopTFTEC.Admin.Context;

namespace ShopTFTEC.Admin.Services;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedUserRoleInitial(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        if (! await _roleManager.RoleExistsAsync("Cliente"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Cliente";
            role.NormalizedName = "CLIENTE";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Admin";
            role.NormalizedName = "ADMIN";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Gerente"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Gerente";
            role.NormalizedName = "GERENTE";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

    }

    public async Task SeedUsersAsync()
    {
        if (await _userManager.FindByEmailAsync("cliente@tftec.com.br") == null)
        {
			ApplicationUser user = new ApplicationUser();
            user.UserName = "cliente.tftec";
            user.Email = "cliente@tftec.com.br";
            user.NormalizedUserName = "CLIENTE.TFTEC";
            user.NormalizedEmail = "CLIENTE@TFTEC.COM.BR";
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            user.SecurityStamp = Guid.NewGuid().ToString();

            IdentityResult result = await _userManager.CreateAsync(user, "Partiunuvem@123");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Cliente");
            }
        }

        if (await _userManager.FindByEmailAsync("admin.tftec@tftec.com.br") == null)
        {
			ApplicationUser user = new ApplicationUser();
            user.UserName = "admin.tftec@tftec.com.br";
            user.Email = "admin.tftec@tftec.com.br";
            user.NormalizedUserName = "ADMIN.TFTEC";
            user.NormalizedEmail = "ADMIN.TFTEC@TFTEC.COM.BR";
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            user.SecurityStamp = Guid.NewGuid().ToString();

            IdentityResult result = await _userManager.CreateAsync(user, "Numsey#2023");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}

