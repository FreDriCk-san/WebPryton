using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.Middleware
{
    public class Unauthorized
    {
        private readonly RequestDelegate Next;

        public Unauthorized(RequestDelegate next)
        {
            this.Next = next;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var content = await reader.ReadToEndAsync();

                    if (context.Request.Headers["WWW-Authenticate"] != CheckId())
                    {
                        throw new Exception("Incorrect authenticate!");
                    }

                    Util.MultipleActions.TransferRequest(context, content);
                }
                await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                await Util.MultipleActions.HandleExceptionAsync(context, exception, HttpStatusCode.Unauthorized);
            }

        }



        private static string CheckId()
        {
            return "Id";
        }

    }
}
