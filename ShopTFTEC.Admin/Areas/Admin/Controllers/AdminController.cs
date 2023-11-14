using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ShopTFTEC.Admin.Areas.Admin.Controllers;

[SecurityHeaders]
[Area("Admin")]
[Authorize]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
