using icy2.idsvr.ResponseHandling.Interface;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using icy2.idsvr.ResponseHandling.Model;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Services;
using Kiwi.Common.Logging;

namespace icy2.idsvr.ResponseHandling
{
    public class ExtendedTokenGenerator : IExtendedTokenGenerator
    {
        private readonly IKeyMaterialService _keys;
        private readonly IKiwiLogger _logger;

        public ExtendedTokenGenerator(IKeyMaterialService keys, IKiwiLogger logger)
        {
            _keys = keys;
            _logger = logger;
        }

        public async Task<string> ProcessAsync(ExtendTokenRequest extendTokenRequest)
        {

            _logger.LogDebug("Creating new JWT Token");
            var handler = new JwtSecurityTokenHandler();
            //Receieved JWT Token
            var rjwt = handler.ReadJwtToken(extendTokenRequest.JWTTokenRAW);
            var claims = rjwt.Claims.ToList();

            //Adding the new api scope to token
            foreach(var aud in extendTokenRequest.AudiencesToAdd)
            {
                //If claims exist, do not add
                var isClaimExists = claims.Where(c => c.Type == "aud" && c.Value == aud).Any();
                if (!isClaimExists)
                    claims.Add(new Claim("aud", aud));
            }

            //Getting keys
            var validationKey = _keys.GetValidationKeysAsync().Result.FirstOrDefault();
            var credentials = new SigningCredentials(validationKey, "RS256");

            JwtSecurityToken newjwt = new JwtSecurityToken(rjwt.Issuer, 
                                                            null, 
                                                            claims, 
                                                            rjwt.ValidFrom, 
                                                            rjwt.ValidTo,
                                                            credentials);         

            var signedAndEncodedToken = handler.WriteToken(newjwt);

            return signedAndEncodedToken;
        }
    }
}
