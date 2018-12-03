using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.Middleware
{
    public class Forbidden
    {
        private readonly RequestDelegate Next;

        public Forbidden(RequestDelegate next)
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

                    var data = JsonConvert.DeserializeObject<Models.Account>(content);

                    if (data.Status != CheckStatus())
                    {
                        throw new Exception("No access rights!");
                    }

                    Util.MultipleActions.TransferRequest(context, content);
                }
                await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                await Util.MultipleActions.HandleExceptionAsync(context, exception, HttpStatusCode.Forbidden);
            }

        }



        private static string CheckStatus()
        {
            return "Admin";
        }

    }
}
