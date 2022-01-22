namespace DBEntity.Models
{
    public class Log
    {
        public int ID { get; set; }
        public string? Exception { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionSource { get; set; }
        public string? ExceptionStackTree { get; set; }
        public int? ExceptionHResult { get; set; }
        public string? ExceptionData { get; set; }
        public string? ExceptionTargetSite { get; set; }
        public string? ExceptionHelpLink { get; set; }
        public string? InnerException { get; set; }
        public string? InnerExceptionMessage { get; set; }
        public string? InnerExceptionSource { get; set; }
        public string? InnerExceptionStackTree { get; set; }
        public int? InnerExceptionHResult { get; set; }
        public string? InnerExceptionData { get; set; }
        public string? InnerExceptionTargetSite { get; set; }
        public string? InnerExceptionHelpLink { get; set; }
        public DateTime OccurredDate { get; set; }
    }
}
