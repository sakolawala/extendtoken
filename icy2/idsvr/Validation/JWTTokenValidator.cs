using icy2.idsvr.Extensions;
using icy2.idsvr.Validation.Interface;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Kiwi.Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation
{
    public class JWTTokenValidator : IJWTTokenValidator
    {
        private readonly IKiwiLogger _logger;
        private readonly IKeyMaterialService _keys;
        private readonly IdentityServerOptions _options;
        private readonly IClientStore _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="BearerTokenUsageValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public JWTTokenValidator(
                    IKiwiLogger logger,
                    IKeyMaterialService keys,
                    IClientStore clients,
                    IdentityServerOptions options)
        {
            _logger = logger;
            _keys = keys;
            _options = options;
            _clients = clients;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<TokenValidationResult> ValidateAsync(HttpContext context)
        {
            var result = await ValidateJWTStructure(context);
            if (!result.IsError)
            {
                string jwtToken = result.Jwt;
                result = await ValidateJWTSignature(jwtToken);
            }
            return result;
        }

        /// <summary>
        /// Validates the JWT Structure for correctness.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<TokenValidationResult> ValidateJWTStructure(HttpContext context)
        {
            TokenValidationResult result
                = new TokenValidationResult();

            _logger.LogDebug("Started JWTToken Validation");
            var body = await context.Request.ReadFormAsync();
            if (body != null)
            {
                var jwttoken = body["jwttoken"].FirstOrDefault();

                if (jwttoken.IsPresent())
                {
                    var printablejwt = GetPrintableJWT(jwttoken);
                    _logger.LogDebug($"JWT Token found (truncated): {printablejwt}");

                    if (jwttoken.Contains("."))
                    {
                        if (jwttoken.Length > _options.InputLengthRestrictions.Jwt)
                        {
                            _logger.LogWarn("JWT too long");

                            result = new TokenValidationResult
                            {
                                IsError = true,
                                Error = OidcConstants.ProtectedResourceErrors.InvalidToken,
                                ErrorDescription = "Token too long"
                            };
                        }
                        else
                        {
                            result = new TokenValidationResult
                            {
                                IsError = false,
                                Jwt = jwttoken
                            };
                        }
                    }
                    else
                    {
                        _logger.LogWarn("Invalid Token Format");
                        result = new TokenValidationResult
                        {
                            IsError = true,
                            Error = OidcConstants.ProtectedResourceErrors.InvalidToken,
                            ErrorDescription = "Invalid Token Format"
                        };

                    }
                }
                else
                {
                    _logger.LogWarn("JWT Token wasn't found");
                    result.IsError = true;
                    result.Error = "Invalid_Token";
                    result.ErrorDescription = "JWT Token missing";
                }
            }
            return result;
        }

        /// <summary>
        /// Validates the JWT for Signature.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task<TokenValidationResult> ValidateJWTSignature(string jwttoken)
        {
            //Validate JWT Token
            SecurityToken securityToken = null;

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();
            var validationKey = _keys.GetValidationKeysAsync().Result.FirstOrDefault();
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = validationKey
            };

            try
            {
                var id = handler.ValidateToken(jwttoken, validationParameters,
                                                            out securityToken);
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    IsError = true,
                    Error = ex.Message                    
                };
            }
            return new TokenValidationResult
            {
                IsError = false,                
                Jwt = jwttoken
            };
        }

        private string GetPrintableJWT(string jwtToken)
        {
            int n = 5;
            if (jwtToken.Length > ((n * 2) + 2))
            {
                string firstn = jwtToken.Substring(0, n);
                string lastn = jwtToken.Substring(jwtToken.Length - n, n);
                return firstn + "...." + lastn;
            }
            else
                return jwtToken;
        }       
    }

}
