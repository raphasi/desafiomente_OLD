using Microsoft.AspNetCore.Identity;
using ShopTFTEC.Admin.Context;
using System.Security.Claims;

namespace ShopTFTEC.Admin.Services;

public class SeedUserClaimsInitial : ISeedUserClaimsInitial
{
    private readonly UserManager<ApplicationUser> _userManager;
    public SeedUserClaimsInitial(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SeedUserClaims()
    {
        try
        {
			// usuário 2
			ApplicationUser user2 = await _userManager.FindByEmailAsync("cliente@tftec.com.br");
            if (user2 is not null)
            {
                var claimList = (await _userManager.GetClaimsAsync(user2))
                                                   .Select(p => p.Type);

                if (!claimList.Contains("CadastradoEm"))
                {
                    var claimResult1 = await _userManager.AddClaimAsync(user2,
                             new Claim("CadastradoEm", "01/01/2020"));
                }
            }
			//usuário 3
			ApplicationUser user3 = await _userManager.FindByEmailAsync("admin.tftec@tftec.com.br");
            if (user3 is not null)
            {
                var claimList = (await _userManager.GetClaimsAsync(user3))
                                                   .Select(p => p.Type);

                if (!claimList.Contains("CadastradoEm"))
                {
                    var claimResult1 = await _userManager.AddClaimAsync(user3,
                             new Claim("CadastradoEm", "02/02/2017"));
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
