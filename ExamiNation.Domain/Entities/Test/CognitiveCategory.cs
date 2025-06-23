using ExamiNation.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamiNation.Domain.Entities.Test
{
    public class CognitiveCategory
    {
        [Key]
        public Guid Id { get;  set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Required]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))]
        public virtual TestType TestType { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
