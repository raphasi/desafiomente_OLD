using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServerHost.Quickstart.UI;
using ShopTFTEC.Admin.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopTFTEC.Admin.Models;

namespace ShopTFTEC.Admin.Controllers
{
    [SecurityHeaders]
    [Area("Admin")]
    [Authorize]
    public class AdminCategoriesController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminCategoriesController(IProductService productService,
                                ICategoryService categoryService,
                                IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> Index()
        {

            var result = await _categoryService.GetAllCategories(await GetAccessToken());

            if (result is null)
                return View("Error");

            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(await _categoryService.GetAllCategories(await GetAccessToken()), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.Create(productVM, await GetAccessToken());

                if (result != null)
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.CategoryId = new SelectList(await
                                     _categoryService.GetAllCategories(await GetAccessToken()), "CategoryId", "Name");
            }
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel catVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.Update(catVM, await GetAccessToken());

                if (result is not null)
                    return RedirectToAction(nameof(Index));
            }
            return View(catVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetCategoryById(id, await GetAccessToken());

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult<CategoryViewModel>> Delete(int id)
        {
            var result = await _categoryService.GetCategoryById(id, await GetAccessToken());

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpPost(), ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryService.DeleteCategorytById(id, await GetAccessToken());

            if (!result)
                return View("Error");

            return RedirectToAction("Index");
        }
        private async Task<string> GetAccessToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
