using Microsoft.AspNetCore.Authorization;

namespace ShopTFTEC.Admin.Policies;

public class TempoCadastroRequirement : IAuthorizationRequirement
{
    public int TempoCadastroMinimo { get; }

    public TempoCadastroRequirement(int tempoCadastroMinimo)
    {
        TempoCadastroMinimo = tempoCadastroMinimo;
    }
}
