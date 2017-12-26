using System.Threading.Tasks;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Validation;
using IdentityModel;
using icy2.idsvr.Validation;
using icy2.idsvr.Utility;
using icy2.idsvr.Endpoints.Results;
using icy2.idsvr.Validation.Interface;
using icy2.idsvr.Extensions;
using Kiwi.Common.Logging;
using icy2.idsvr.ResponseHandling.Interface;
using icy2.idsvr.ResponseHandling.Model;
using System.Linq;
using static icy2.idsvr.Endpoints.Results.CustomValidationErrorResult;

namespace icy2.idsvr.Endpoints
{
    public class CustomExtendEndpoint : IEndpointHandler
    {
        private readonly IKiwiLogger _logger;
        private readonly IClientSecretValidator _clientValidator;
        private readonly IGrantTypeValidator _grantTypeValidator;
        private readonly IAdditonalAudienceValidator _additonalAudienceValidator;
        private readonly IJWTTokenValidator _jwtTokenValidator;
        private readonly IExtendedTokenGenerator _tokenGenerator;


        public CustomExtendEndpoint(IKiwiLogger kiwiLogger, 
                    IClientSecretValidator clientValidator,
                    IGrantTypeValidator grantTypeValidator,
                    IAdditonalAudienceValidator additonalAudienceValidator, 
                    IJWTTokenValidator jwtTokenValidator,
                    IExtendedTokenGenerator tokenGenerator)
        {
            _logger = kiwiLogger;           
            _clientValidator = new InternalServiceClientValidator(kiwiLogger, clientValidator);
            _grantTypeValidator = grantTypeValidator;
            _additonalAudienceValidator = additonalAudienceValidator;
            _jwtTokenValidator = jwtTokenValidator;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogDebug("Processing token request.");

            // validate HTTP
            if (context.Request.Method != "POST")
            {
                _logger.LogWarn("Invalid HTTP request for token endpoint");            
                return Error("invalid_request", "Request isn't a HTTP POST request");               
            }
            //In not a POST type request
            if (!context.Request.HasFormContentType)
            {
                _logger.LogWarn("HTTP request doesn't have FormContentType");
                return Error("invalid_request", "Request isn't a Form Content Type");
            }

            return await ProcessTokenRequestAsync(context);
        }

        private async Task<IEndpointResult> ProcessTokenRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Start validating token request.");

            ExtendTokenValidationResult validationResult = await ValidateRequest(context);
            if (validationResult.IsError)
                return Error(validationResult.Error);

            _logger.LogDebug("Started Processing Token...");

            ExtendTokenRequest tokenRequest = await FormTokenRequest(context);

            var newToken = await _tokenGenerator.ProcessAsync(tokenRequest);

            _logger.LogDebug("End Processing Token");
            return new ExtendedTokenResult(newToken);            
        }

        private async Task<ExtendTokenValidationResult> ValidateRequest(HttpContext context)
        {            
            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null)
            {
                return new ExtendTokenValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.TokenErrors.InvalidClient
                };                
            }

            // validate Grant Type
            var grantType = await _grantTypeValidator.ValidateAsync(context);
            if (grantType.Error.IsPresent())
            {
                return new ExtendTokenValidationResult
                {
                    IsError = true,
                    Error = OidcConstants.TokenErrors.InvalidGrant
                };
            }

            // validate additional audience
            var addauds = await _additonalAudienceValidator.ValidateAsync(context);
            if (addauds.IsError)
            {
                return new ExtendTokenValidationResult
                {
                    IsError = true,
                    Error = addauds.Error                 
                };
            }

            // validate request
            var jwtTokenResult = await _jwtTokenValidator.ValidateAsync(context);
            if (jwtTokenResult.IsError)
            {
                return new ExtendTokenValidationResult
                {
                    IsError = true,
                    Error = jwtTokenResult.Error
                };
            }
            else
            {
                return new ExtendTokenValidationResult
                {
                    IsError = false,
                    JWTTokenRAW = jwtTokenResult.Jwt
                };
            }
        }

        private async Task<ExtendTokenRequest> FormTokenRequest(HttpContext context)
        {
            ExtendTokenRequest rtnRequest = new ExtendTokenRequest();
            var body = await context.Request.ReadFormAsync();
            if (body != null)
            {
                var token = body[ExtentTokenConstants.jwttoken].FirstOrDefault();
                var additionalAud = body[ExtentTokenConstants.additionalAudience].FirstOrDefault();

                var auds = additionalAud.Split(",").Select(c => c.Trim()).ToList();

                rtnRequest = new ExtendTokenRequest
                {
                    JWTTokenRAW = token,
                    AudiencesToAdd = auds
                };
            }
            return rtnRequest;
        }

        private CustomValidationErrorResult Error(string error, string errorDescription = null)
        {
            return new CustomValidationErrorResult(error, errorDescription);
        }
    }
}
