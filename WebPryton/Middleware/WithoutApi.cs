using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPryton.Middleware
{
    public class WithoutApi
    {
        //private readonly RequestDelegate Next;

        public WithoutApi(RequestDelegate next)
        {
            //this.Next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync("Api version is not entered!");
        }

    }
}
