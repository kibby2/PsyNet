namespace PsyNet.Web.Models.Domain
{
    public class BlogReport
    {
        public Guid Id { get; set; }
        public Guid BlogPostId { get; set; }
        public string ReporterUserId { get; set; }
        public string Comment { get; set; }
        public DateTime ReportDate { get; set; }
        public bool Resolved { get; set; }

        // Navigation properties
        public BlogPost BlogPost { get; set; }
    }
}