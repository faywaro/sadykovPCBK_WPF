using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykovPCBKpartner.Models
{
    /// <summary>
    /// Запись истории реализации продукции партнёром
    /// </summary>
    [Table("partner_sales", Schema = "app")]
    public class PartnerSale
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("sale_date")]
        public DateTime SaleDate { get; set; } = DateTime.Today;

        // Навигационные свойства
        [ForeignKey("PartnerId")]
        public virtual Partner? Partner { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
