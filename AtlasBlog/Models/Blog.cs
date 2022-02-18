using System.ComponentModel.DataAnnotations;

namespace AtlasBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Blog Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength =2)]
        public string BlogName { get; set; } = "blog";

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 2)]
        public string BlogDescription { get; set; } = "desc";

        [DataType(DataType.Date)] 
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
        //public Nullable<DateTime> DateUpdated { get; set; }


        //I want to store an image for this Blog
        [Display(Name = "Choose Image")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ImageType { get; set; } = "";


        //List of Posts as children
        public ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}
