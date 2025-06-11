namespace PsyNet.Web.Models.ViewModels
{
    public class ReportCommentViewModel
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; }
        public Guid BlogPostId { get; set; }
        public string BlogPostTitle { get; set; }
        public string CommentContent { get; set; }
        public string CommentAuthor { get; set; }
    }
}