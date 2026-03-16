using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykovPCBKpartner.Models
{
    /// <summary>
    /// Модель партнёра компании
    /// </summary>
    [Table("partners", Schema = "app")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("company_name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("legal_address")]
        public string LegalAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(12)]
        [Column("inn")]
        public string Inn { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("director_name")]
        public string DirectorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("rating")]
        public int Rating { get; set; } = 0;

        [MaxLength(500)]
        [Column("logo_path")]
        public string? LogoPath { get; set; }

        // Навигационные свойства
        [ForeignKey("TypeId")]
        public virtual PartnerType? PartnerType { get; set; }

        public virtual ICollection<PartnerSale> Sales { get; set; } = new List<PartnerSale>();
    }
}
