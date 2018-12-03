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
    public class NotFound
    {
        private readonly RequestDelegate Next;

        public NotFound(RequestDelegate next)
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

                    if (!FindLogin(data.Login) || !FindPassword(data.Password))
                    {
                        throw new Exception("Not found!");
                    }

                    Util.MultipleActions.TransferRequest(context, content);
                }
                await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                await Util.MultipleActions.HandleExceptionAsync(context, exception, HttpStatusCode.NotFound);
            }

        }



        private static bool FindLogin(string login)
        {
            if (login == "Login")
            {
                return true;
            }

            return false;
        }



        private static bool FindPassword(long password)
        {
            if (password == 123)
            {
                return true;
            }

            return false;
        }

    }
}
