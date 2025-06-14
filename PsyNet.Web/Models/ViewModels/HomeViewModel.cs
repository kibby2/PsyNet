using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

        public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalBlogs { get; set; }
        public int PageSize { get; set; } = 3;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
