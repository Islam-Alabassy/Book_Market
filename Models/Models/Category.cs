using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        [DisplayName("Category Name")]
        public required string Name { get; set; }
        [Display(Name="Display Order")]
        [Range(1,100)]
        public int DisplayOrder { get; set; }
    }
}
