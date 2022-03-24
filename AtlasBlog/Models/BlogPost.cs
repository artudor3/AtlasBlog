using AtlasBlog.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtlasBlog.Models
{
    public class BlogPost
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Blog Foreign Key
        /// </summary>
        [Display(Name = "Blog Id")]
        public int BlogId { get; set; }

        /// <summary>
        /// The Blog Title
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 3)]
        public string Title { get; set; } = "";

        /// <summary>
        /// The Slug is derived from the Title and must be unique
        /// </summary>
        public string Slug { get; set; } = "";

        [Display(Name = "Mark for deletion")]
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 3)]
        public string Abstract { get; set; } = "";

        [Display(Name = "Blog Post State")]
        public BlogPostState BlogPostState { get; set; }

        [Required]
        public string Body { get; set; } = "";

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //Navigation properties
        public virtual Blog? Blog { get; set; }
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    }
}
