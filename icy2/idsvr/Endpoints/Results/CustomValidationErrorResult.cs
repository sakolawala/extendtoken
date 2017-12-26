using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.Endpoints.Results
{
    public class CustomValidationErrorResult : IdentityServer4.Hosting.IEndpointResult
    {
        string _error;
        string _errorDescription;

        public CustomValidationErrorResult(string error, string errorDescription)
        {
            if (string.IsNullOrEmpty(error)) throw new ArgumentNullException("Error must be set");
            _error = error;
            _errorDescription = errorDescription;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                error = _error,
                error_description = _errorDescription
            };

           
            await context.Response.WriteJsonAsync(dto);
           
        }

        internal class ResultDto
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
    }
}
