using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPryton
{
    public static class StatusExtensions
    {

        public static IApplicationBuilder CheckBadRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.BadRequest>();
        }



        public static IApplicationBuilder CheckUnauthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.Unauthorized>();
        }

        
        
        public static IApplicationBuilder CheckForbidden(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.Forbidden>();
        }



        public static IApplicationBuilder CheckNotFound(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.NotFound>();
        }



        public static IApplicationBuilder UseOK(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.OK>();
        }
    }
}
