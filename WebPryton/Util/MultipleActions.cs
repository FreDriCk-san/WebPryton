using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.Util
{
    public class MultipleActions
    {

        public static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpStatusCode)
        {
            var response = context.Response;
            var message = exception.Message;
            var description = httpStatusCode.ToString();

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;
            await response.WriteAsync(JsonConvert.SerializeObject(new CustomResponse
            {
                Message = message,
                Description = description
            }));
        }



        public static void TransferRequest(HttpContext context, string content)
        {
            var request = context.Request;
            var requestData = Encoding.UTF8.GetBytes(content);
            request.Body = new MemoryStream(requestData);
        }
    }
}
