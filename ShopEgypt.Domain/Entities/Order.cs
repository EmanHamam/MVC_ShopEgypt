using ShopEgypt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEgypt.Domain.Entities;

[Table("Order")]
public partial class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(450)]
    public string ApplicationUserId { get; set; }

    public DateTime OrderDate { get; set; }

    [Required]
    [StringLength(50)]
    public OrderStatus Status { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    public Address ShippingAddress { get; set; }

    [ForeignKey("ApplicationUserId")]
    [InverseProperty("Orders")]
    public virtual ApplicationUser ApplicationUser { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual Payment Payment { get; set; }
}
