using System.ComponentModel.DataAnnotations;

namespace AbeServices.AttributeAuthority.Models.ViewModels
{
    public class CreateLoginViewModel
    {
        [Required]
        [MinLength(5)]
        [StringLength(1000)]
        public string Login { get; set; }

        [Required]
        [MinLength(5)]
        [StringLength(1000)]
        public string SharedKey { get; set; }

        [MaxLength(1000)]
        public string[] Attributes { get; set; }
    }
}