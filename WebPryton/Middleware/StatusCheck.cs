using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var statusCode = HttpStatusCode.Created;
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            try
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var content = await reader.ReadToEndAsync();

                    // Bad Request - 400

                    var accountProperties = typeof(Models.Account).GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.Static).ToList();

                    var accountJSON = (JObject)JsonConvert.DeserializeObject(content);

                    // Check, if count of model properties is the same as json properties
                    if (accountJSON.Count != accountProperties.Count)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new Exception($"Count of properties:{accountJSON.Count}, Need:{accountProperties.Count}");
                    }

                    foreach (var pair in accountJSON)
                    {
                        // Check if JToken is empty
                        if (JTokenIsNullOrEmpty(pair.Value))
                        {
                            statusCode = HttpStatusCode.BadRequest;
                            throw new Exception($"{pair.Key} value is null!");
                        }

                        // Check if JToken is the same type as a model property (here Models.Account) 
                        foreach (var field in accountProperties)
                        {
                            if (pair.Key == field.Name)
                            {
                                if (pair.Value.ToObject<object>().GetType() != field.PropertyType)
                                {
                                    statusCode = HttpStatusCode.BadRequest;
                                    throw new Exception($"{pair.Key} have incorrect type! {pair.Value.ToObject<object>().GetType()} != {field.PropertyType}");
                                }
                            }
                        }
                    }

                    // Unauthorized - 401

                    if (context.Request.Headers["WWW-Authenticate"] != CheckId())
                    {
                        statusCode = HttpStatusCode.Unauthorized;
                        throw new Exception("Incorrect authenticate!");
                    }

                    // Forbidden - 403

                    var data = JsonConvert.DeserializeObject<Models.Account>(content);

                    if (data.Status != CheckStatus())
                    {
                        statusCode = HttpStatusCode.Forbidden;
                        throw new Exception("No access rights!");
                    }

                    // Not found - 404

                    if (!FindLogin(data.Login) || !FindPassword(data.Password))
                    {
                        statusCode = HttpStatusCode.NotFound;
                        throw new Exception("Not found!");
                    }


                }

                await Next.Invoke(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception, statusCode);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpStatusCode)
        {
            var response = context.Response;
            var message = exception.Message;
            var description = httpStatusCode.ToString();

            response.ContentType = "application/json";
            response.StatusCode = context.Response.StatusCode;
            await response.WriteAsync(JsonConvert.SerializeObject(new CustomResponse
            {
                StatusCode = (int)httpStatusCode,
                Message = message,
                Description = description
            }));
        }


        private bool JTokenIsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }



        private static string CheckId()
        {
            return "Id";
        }


        private static string CheckStatus()
        {
            return "Admin";
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

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class StatusCheckExtensions
    {
        public static IApplicationBuilder UseStatusCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StatusCheck>();
        }
    }
}
