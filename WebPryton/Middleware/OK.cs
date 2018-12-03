using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.Middleware
{
    public class OK
    {
        private readonly RequestDelegate Next;

        public OK(RequestDelegate next)
        {
            this.Next = next;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;

            await response.WriteAsync(JsonConvert.SerializeObject(new CustomResponse
            {
                Message = "OK!",
                Description = "Content was proceed..."
            }));

            
        }

    }
}
