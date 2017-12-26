using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Kiwi.Common.Logging;
using icy2.idsvr.Utility;

namespace icy2.idsvr.Validation
{
    public class InternalServiceClientValidator : IClientSecretValidator
    {
        private readonly IKiwiLogger _logger;
        private readonly IClientSecretValidator _clientValidator;

        public InternalServiceClientValidator(
            IKiwiLogger logger,
            IClientSecretValidator clientValidator
            )
        {
            _logger = logger;
            _clientValidator = clientValidator;
        }

        public async Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            _logger.LogDebug("Start InternalServiceClient Validation");
            //First Check if the client exists
            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);

            if (clientResult.IsError)
                return clientResult;            

            var body = await context.Request.ReadFormAsync();
            if (body != null)
            {
                if (!validateClient(body))
                {
                    return new ClientSecretValidationResult
                    {
                        Error = "invalid_client",
                        ErrorDescription = "This endpoint is only allowed for service clients"
                    };
                }
                else
                {
                    //Client is Internal Service, proceed with request
                    return clientResult;
                }                              
            }
            _logger.LogDebug("Completed InternalServiceClient Validation");
            return null;
        }

        private bool validateClient(IFormCollection body)
        {
            var id = body[ExtentTokenConstants.clientid].FirstOrDefault();
            _logger.LogDebug($"Client '{id}' found");
            if (id != "InternalServiceClient")
            {
                _logger.LogWarn($"Client isn't 'InternalServiceClient', InternalServiceClientValidator validation failed");
                return false;
            }
            else
                return true;
        }
    }
}
