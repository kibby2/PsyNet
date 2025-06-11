namespace PsyNet.Web.Models.ViewModels
{
    public class CommentReportsViewModel
    {
        public IEnumerable<CommentReportItem> Reports { get; set; }
    }

    public class CommentReportItem
    {
        public Guid ReportId { get; set; }
        public Guid CommentId { get; set; }
        public Guid BlogPostId { get; set; }
        public string BlogPostTitle { get; set; }
        public string BlogUrlHandle { get; set; }
        public string CommentContent { get; set; }
        public string CommentAuthor { get; set; }
        public string ReporterName { get; set; }
        public string ReportComment { get; set; }
        public DateTime ReportDate { get; set; }
    }
}