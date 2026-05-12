using System.ComponentModel.DataAnnotations;

namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class AddressDTO
    {
        [Required(ErrorMessage = "Street is required.")]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Display(Name = "State / Governorate")]
        public string? State { get; set; }

        [Display(Name = "ZIP / Postal Code")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
    }
}
