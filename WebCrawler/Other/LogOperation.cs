using DBEntity.Context;
using DBEntity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebCrawler.Other
{
    internal class LogOperation
    {
        private string LogHtml { get; set; }
        private StreamWriter swLogger { get; set; }
        public LogOperation()
        {
            LogHtml = "<table class=\"table table-bordered col-sm-11\" style=\"margin: 2%;\">" +
"<tr class=\"d-flex\"><th class=\"col-1\">Occurred Date</th><td class=\"col-11\">{16}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception</th><td class=\"col-11\">{0}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Message</th><td class=\"col-11\">{1}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Source</th><td class=\"col-11\">{2}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Stack Tree</th><td class=\"col-11\">{3}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception HResult</th><td class=\"col-11\">{4}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Data</th><td class=\"col-11\">{5}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Target Site</th><td class=\"col-11\">{6}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Exception Help Link</th><td class=\"col-11\">{7}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception</th><td class=\"col-11\">{8}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Message</th><td class=\"col-11\">{9}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Source</th><td class=\"col-11\">{10}" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Stack Tree</th><td class=\"col-11\">{11}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception HResult</th><td class=\"col-11\">{12}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Data</th><td class=\"col-11\">{13}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Target Site</th><td class=\"col-11\">{14}</td></tr>" +
"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Help Link</th><td class=\"col-11\">{15}</td></tr>" +
"</table><hr>";
            swLogger = new($"{DateTime.Now.ToString("yyyyMMddHHmmss")}.html", append: true);
            swLogger.WriteLine("<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\" integrity=\"sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T\" crossorigin=\"anonymous\">");
            swLogger.Flush();
        }
        public void InsertLog(Exception exception)
        {
            using (CrawlerContext context = new())
            {
                Log log = new()
                {
                    Exception = exception?.GetType().Name,
                    ExceptionMessage = exception?.Message,
                    ExceptionSource = exception?.Source,
                    ExceptionStackTree = exception?.StackTrace,
                    ExceptionHResult = exception?.HResult,
                    ExceptionData = exception?.Data.ToString(),
                    ExceptionTargetSite = exception?.TargetSite?.Name,
                    ExceptionHelpLink = exception?.HelpLink,
                    InnerException = exception?.InnerException?.GetType().Name,
                    InnerExceptionMessage = exception?.InnerException?.Message,
                    InnerExceptionSource = exception?.InnerException?.Source,
                    InnerExceptionStackTree = exception?.InnerException?.StackTrace,
                    InnerExceptionHResult = exception?.InnerException?.HResult,
                    InnerExceptionData = exception?.InnerException?.Data.ToString(),
                    InnerExceptionTargetSite = exception?.InnerException?.TargetSite?.Name,
                    InnerExceptionHelpLink = exception?.InnerException?.HelpLink,
                };
                context.Log.Add(log);
                context.SaveChanges();
            }
        }

        public void ExportLog()
        {
            using (CrawlerContext context = new())
            {
                List<Log> lstLogs =  context.Log.OrderByDescending(p => p.OccurredDate).ToList();
                foreach (Log log in lstLogs)
                {
                    swLogger.WriteLine(string.Format(LogHtml, log.Exception, log.ExceptionMessage, log.ExceptionSource, log.ExceptionStackTree, log.ExceptionHResult, log.ExceptionData,
                        log.ExceptionTargetSite, log.ExceptionHelpLink, log.InnerException, log.InnerExceptionMessage, log.InnerExceptionSource, log.InnerExceptionStackTree,
                        log.InnerExceptionHResult, log.InnerExceptionData, log.InnerExceptionTargetSite, log.InnerExceptionHelpLink, log.OccurredDate));
                    swLogger.Flush();
                }
            }
        }

    }
}
