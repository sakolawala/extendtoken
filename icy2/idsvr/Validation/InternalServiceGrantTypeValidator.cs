using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using icy2.idsvr.Extensions;
using icy2.idsvr.Validation.Interface;
using icy2.idsvr.Endpoints.Results;
using Kiwi.Common.Logging;

namespace icy2.idsvr.Validation
{
    public class InternalServiceGrantTypeValidator : IGrantTypeValidator
    {
        private readonly IKiwiLogger _logger;

        public InternalServiceGrantTypeValidator(IKiwiLogger logger)
        {
            _logger = logger;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(HttpContext context)
        {
            _logger.LogDebug("Start InternalServiceGrantType Validation");        

            var body = await context.Request.ReadFormAsync();
            if (body != null)
            {
                string grantType;
                if (!validateGrantType(body, out grantType))
                {
                    return new CustomGrantValidationResult
                    {                        
                        Error = "invalid_grant",
                        ErrorDescription = "Invalid GrantType"
                    };
                }
                else
                {
                    return new CustomGrantValidationResult
                    {
                        GrantType = grantType
                    };
                }                              
            }
            _logger.LogDebug("Completed InternalServiceGrantType Validation");
            return null;
        }

        private bool validateGrantType(IFormCollection body, out string grantType)
        {
            grantType = body["grant_type"].FirstOrDefault();
            _logger.LogDebug($"GrantType '{grantType}' found");
            if (grantType != "extend_jwt_token")
            {
                _logger.LogWarn("GrantType isn't 'extend_jwt_token', InternalServiceGrantTypeValidator validation failed");
                return false;
            }
            else
                return true;
        }
    }
}
