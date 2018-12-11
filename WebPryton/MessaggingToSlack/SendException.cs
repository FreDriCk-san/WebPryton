using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.MessaggingToSlack
{
    public class SendException
    {
        private class Info
        {
            public string channel { get; set; }
            public string text { get; set; }
        }


        public static void Send(Exception exception)
        {
            var send = Task.Run(async () =>
            {
                await SendAsync(exception);
            });
        }

        public static async Task SendAsync(Exception exception)
        {
            //var proxy = ProxyActivation.CreateConnection();

            var info = new Info
            {
                channel = Resources.MainChannel,
                text = ExceptionSerializer(exception)
            };

            var infoJSON = JsonConvert.SerializeObject(info);
            var data = Encoding.ASCII.GetBytes(infoJSON);

            var request = (HttpWebRequest)WebRequest.Create("https://slack.com/api/chat.postMessage");
            request.Method = "POST";
            //request.Proxy = proxy;
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Headers.Add($"Authorization: Bearer {Resources.SlackToken}");

            using (var stream = request.GetRequestStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
                stream.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        private static string ExceptionSerializer(Exception exception)
        {
            var exceptionList = exception.FlattenHierarchy().ToList();
            var exceptionBuilder = new StringBuilder();

            foreach (var item in exceptionList)
            {
                var stackTrace = new StackTrace(item, true);
                var stackFrames = stackTrace.GetFrames();

                var traceBuilder = new StringBuilder();

                foreach (var frame in stackFrames)
                {
                    traceBuilder.AppendLine($"\t\t_File Column Number:_ {frame.GetFileColumnNumber()}")
                                .AppendLine($"\t\t_File Line Number:_ {frame.GetFileLineNumber()}")
                                .AppendLine($"\t\t_File Name:_ {frame.GetFileName()}")
                                .AppendLine($"\t\t_Method Name:_ {frame.GetMethod().Name}")
                                .AppendLine("\t\t*/++++++++++++/*");
                }

                exceptionBuilder.AppendLine($"*Date:* {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}")
                                .AppendLine($"*Type:* {item.GetType().FullName}")
                                .AppendLine($"*Message:* {item.Message}")
                                .AppendLine($"*Source:* {item.Source}")
                                .AppendLine($"*Help:* https://stackoverflow.com/search?q={item.GetType().FullName}")
                                .AppendLine($"*StackTrace:* \n{traceBuilder.ToString()}")
                                .AppendLine("_/-----------------------------/_");
            }

            return exceptionBuilder.ToString();
        }
    }

    static class Extensions
    {
        public static IEnumerable<Exception> FlattenHierarchy(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("Exception is null!");
            }

            do
            {
                yield return exception;
                exception = exception.InnerException;
            }
            while (exception != null);
        }
    }
}
