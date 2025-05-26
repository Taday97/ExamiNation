using ExamiNation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.CognitiveCategory
{
    public class CreateCognitiveCategoryDto
    {

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

        public Guid? CognitiveCategoryId { get; set; }
    }
}
