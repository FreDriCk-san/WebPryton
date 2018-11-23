using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebPryton.Middleware
{
    public class BadRequest
    {
        private readonly RequestDelegate Next;



        public BadRequest(RequestDelegate next)
        {
            this.Next = next;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var content = reader.ReadToEnd();

                    var accountProperties = typeof(Models.Account).GetProperties(
                        BindingFlags.FlattenHierarchy |
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.Static).ToList();

                    var accountJSON = (JObject)JsonConvert.DeserializeObject(content);

                    // Check, if count of model properties is the same as json properties
                    if (accountJSON.Count != accountProperties.Count)
                    {
                        throw new Exception($"Count of properties:{accountJSON.Count}, Need:{accountProperties.Count}");
                    }

                    foreach (var pair in accountJSON)
                    {
                        // Check if JToken is empty
                        if (JTokenIsNullOrEmpty(pair.Value))
                        {
                            throw new Exception($"{pair.Key} value is null!");
                        }

                        // Check if JToken is the same type as a model property (here Models.Account) 
                        foreach (var field in accountProperties)
                        {
                            if (pair.Key == field.Name)
                            {
                                if (pair.Value.ToObject<object>().GetType() != field.PropertyType)
                                {
                                    throw new Exception($"{pair.Key} have incorrect type! {pair.Value.ToObject<object>().GetType()} != {field.PropertyType}");
                                }
                            }
                        }
                    }
                }

                await Next.Invoke(context);
            }
            catch(Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }



        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            var statusCode = (int)HttpStatusCode.BadRequest;
            var message = exception.Message;
            var description = HttpStatusCode.BadRequest.ToString();

            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            await response.WriteAsync(JsonConvert.SerializeObject(new CustomErrorResponse
            {
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
    }
}
