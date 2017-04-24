using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ExigoIntegration.Models
{
    public class ErrorLog
    {
        public static void LogError(Exception ex)
        {
            try
            {
                // You could use any logging approach here

                StringBuilder builder = new StringBuilder();
                builder
                    .AppendLine("----------")
                    .AppendLine(DateTime.Now.ToString())
                    .AppendFormat("Source:\t{0}", ex.Source)
                    .AppendLine()
                    .AppendFormat("Target:\t{0}", ex.TargetSite)
                    .AppendLine()
                    .AppendFormat("Type:\t{0}", ex.GetType().Name)
                    .AppendLine()
                    .AppendFormat("Message:\t{0}", ex.Message)
                    .AppendLine()
                    .AppendFormat("Stack:\t{0}", ex.StackTrace)
                    .AppendLine();

                string filePath = HttpContext.Current.Server.MapPath("~/Error-" + DateTime.Now.ToString("MM-yyyy") + ".log");

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(builder.ToString());
                    writer.Flush();
                }
            }
            catch (Exception Ex)
            {
            }
        }

        public static void LogInfo(string Message)
        {
            try
            {
                // You could use any logging approach here

                StringBuilder builder = new StringBuilder();
                builder
                    .AppendLine("----------")
                    .AppendLine(DateTime.Now.ToString())
                    .AppendFormat("Message:\t{0}", Message)
                    .AppendLine()
                    .AppendFormat("Time:\t{0}", DateTime.Now)
                    .AppendLine();

                string filePath = HttpContext.Current.Server.MapPath("~/Error-" + DateTime.Now.ToString("MM-yyyy") + ".log");

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(builder.ToString());
                    writer.Flush();
                }
            }
            catch (Exception Ex)
            {
            }
        }
    }
}
