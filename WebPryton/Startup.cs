using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebPryton
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();


            // For api version 1.0 (Status Codes: 200, 400, 500 etc)
            app.MapWhen(context => context.Request.Query["api"] == "v1", firstApi =>
            {
                firstApi.CheckBadRequest();
                firstApi.CheckUnauthorized();
                firstApi.CheckForbidden();
                firstApi.CheckNotFound();
                firstApi.UseOK();
            });


            // For api version 2.0 (Status Code: 200 [with full description of execution])
            app.MapWhen(context => context.Request.Query["api"] == "v2", secondApi =>
            {
                // CHANGE!!!

                secondApi.Use(async (context, next) =>
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    response.StatusCode = 200;

                    var badRequest = Task.Run(async () =>
                    {
                        await next.Invoke();
                    });

                    await response.WriteAsync(JsonConvert.SerializeObject(new CustomResponse
                    {
                        Message = "OK!",
                        Description = String.Empty
                    }));

                    
                    await next.Invoke();
                    await next.Invoke();
                    await next.Invoke();
                });

                secondApi.CheckBadRequest();
                secondApi.CheckUnauthorized();
                secondApi.CheckForbidden();
                secondApi.CheckNotFound();
            });



            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
