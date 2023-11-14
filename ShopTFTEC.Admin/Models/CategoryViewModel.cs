using System.ComponentModel.DataAnnotations;

namespace ShopTFTEC.Admin.Models;
public class CategoryViewModel
{
    public int CategoryId { get; set; }
    [Required]
    [Display(Name = "Nome da Categoria")]
    public string? Name { get; set; }
}
