using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebPryton.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class StatusCheck
    {
        private readonly RequestDelegate Next;

        public StatusCheck(RequestDelegate next)
        {
            Next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBody = context.Response.Body;
            var responseBody = String.Empty;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    context.Response.Body = memoryStream;
                    await Next.Invoke(context);

                    memoryStream.Position = 0;

                    responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                }

                var data = JsonConvert.DeserializeObject(responseBody);
                await MakeResponseAsync(context, data, originalBody, (HttpStatusCode)context.Response.StatusCode);
            }
            catch (JsonReaderException jException)
            {
                await MakeResponseAsync(context, jException.Message, originalBody);
            }
            catch (Exception exception)
            {
                await MessaggingToSlack.SendException.SendAsync(exception);
                await MakeResponseAsync(context, exception.Message, originalBody, HttpStatusCode.InternalServerError);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }



        private async Task MakeResponseAsync(HttpContext context, object content, Stream originalBody, HttpStatusCode? httpStatusCode = HttpStatusCode.OK)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";

            var json = JsonConvert.SerializeObject(new CustomResponse
            {
                StatusCode = (int)httpStatusCode,
                Message = content?.ToString(),
                Description = httpStatusCode.ToString()
            });

            var encodeJSON = Encoding.UTF8.GetBytes(json.ToString());
            await originalBody.WriteAsync(encodeJSON);

        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class StatusCheckExtensions
    {
        public static IApplicationBuilder UseStatusCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StatusCheck>();
        }
    }
}
