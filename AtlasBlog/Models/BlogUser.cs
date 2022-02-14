using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasBlog.Models
{
    public class BlogUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? DisplayName { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{LastName}, {FirstName}";
            }
        }

        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Blog Image")]
        public IFormFile? ImageFile { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }
        
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
