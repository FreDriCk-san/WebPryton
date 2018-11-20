using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPryton
{
    public static class StatusExtensions
    {
        public static IApplicationBuilder UseToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StatusMiddleware>();
        }

        public static IApplicationBuilder ApiIsNotUsed(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.WithoutApi>();
        }

        public static IApplicationBuilder CheckBadRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware.BadRequest>();
        }
    }
}
