using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Models.ViewModels
{
    public class BlogReportsViewModel
    {
        public IEnumerable<BlogReportItem> Reports { get; set; }
    }

    public class BlogReportItem
    {
        public Guid ReportId { get; set; }
        public Guid BlogPostId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogAuthor { get; set; }
        public string ReporterName { get; set; }
        public string ReportComment { get; set; }
        public DateTime ReportDate { get; set; }
        public string BlogUrlHandle { get; set; }
    }
}