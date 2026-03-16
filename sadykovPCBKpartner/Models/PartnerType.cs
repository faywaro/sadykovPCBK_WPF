using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykovPCBKpartner.Models
{
    /// <summary>
    /// Тип партнёра (ООО, ЗАО, ПАО и т.д.)
    /// </summary>
    [Table("partner_types", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("type_name")]
        public string TypeName { get; set; } = string.Empty;

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}
