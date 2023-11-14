// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Quickstart.UI
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "O email � obrigat�rio")]
        [EmailAddress(ErrorMessage = "Email inv�lido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A senha � obrigat�ria")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}