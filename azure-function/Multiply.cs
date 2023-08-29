using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace MathPlugin
{
    public class Multiply
    {
        private readonly ILogger _logger;

        public Multiply(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Add>();
        }

        [Function("Multiply")]
        [OpenApiOperation(operationId: "Multiply", tags: new[] { "ExecuteFunction" }, Description = "Multiplies two numbers.")]
        [OpenApiParameter(name: "number1", Description = "The first number to multiply", Required = true, In = ParameterLocation.Query)]
        [OpenApiParameter(name: "number2", Description = "The second number to multiply", Required = true, In = ParameterLocation.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the multiply result of the two numbers.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            bool result1 = double.TryParse(req.Query["number1"], out double number1);
            bool result2 = double.TryParse(req.Query["number2"], out double number2);

            if (result1 && result2)
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain");
                double result = number1 * number2;
                response.WriteString(result.ToString(CultureInfo.CurrentCulture));

                _logger.LogInformation($"Numbers are {number1} and {number2}");
                _logger.LogInformation($"Add function processed a request. Result: {result}");

                return response;
            }
            else
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString("Please pass two numbers on the query string or in the request body");

                return response;
            }
        }
    }
}