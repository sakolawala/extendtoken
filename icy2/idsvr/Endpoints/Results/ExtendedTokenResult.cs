using IdentityServer4.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IdentityModel;

namespace icy2.idsvr.Endpoints.Results
{
    public class ExtendedTokenResult : IEndpointResult
    {
        private readonly string _encodedToken;

        public ExtendedTokenResult(string signedAndEncodedToken)
        {
            _encodedToken = signedAndEncodedToken;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();
            var dto = new
            {
                access_token = _encodedToken,
                token_type = OidcConstants.TokenResponse.BearerTokenType
            };

            await context.Response.WriteJsonAsync(dto);
        }
    }
}
