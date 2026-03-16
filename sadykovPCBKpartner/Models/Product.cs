using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykovPCBKpartner.Models
{
    /// <summary>
    /// Продукция компании
    /// </summary>
    [Table("products", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("article")]
        public string Article { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("product_type")]
        public string ProductType { get; set; } = string.Empty;

        [Column("min_price")]
        public decimal MinPrice { get; set; }

        public virtual ICollection<PartnerSale> Sales { get; set; } = new List<PartnerSale>();
    }
}
