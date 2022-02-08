using System.ComponentModel.DataAnnotations;

namespace AtlasBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public int BlogId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 3)]
        public string Title { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 3)]
        public string Abstract { get; set; } = "";

        [Required]
        public string Body { get; set; } = "";

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }



        //Navigation properties
        public Blog Blog { get; set; } = default!;
    }
}
