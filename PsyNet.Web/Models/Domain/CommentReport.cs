namespace PsyNet.Web.Models.Domain
{
    public class CommentReport
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public string ReporterUserId { get; set; }
        public string Comment { get; set; }
        public DateTime ReportDate { get; set; }
        public bool Resolved { get; set; }
    }
}