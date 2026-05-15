using ShopEgypt.Application.DTOs.Admin;

namespace ShopEgypt.Areas.Adminn.Models.Products
{
    public class AdminProductFormViewModel
    {
        public AdminCreateProductDto CreateDto { get; set; } = new();
        public AdminUpdateProductDto UpdateDto { get; set; } = new();

        public bool IsEdit { get; set; }

        public List<AdminSelectItemViewModel> Categories { get; set; } = new();
        public List<AdminSelectItemViewModel> Brands { get; set; } = new();
    }
}
