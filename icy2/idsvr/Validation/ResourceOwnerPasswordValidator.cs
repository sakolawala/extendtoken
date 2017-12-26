using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation
{
    public class ResourceOwnerPasswordValidator: IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var usrname = context.UserName;
                var pwd = context.Password;
                if (usrname == "alice")
                {
                    context.Result = new GrantValidationResult(
                                            subject: "alice",
                                            authenticationMethod: "custom",
                                            claims: new[] { new Claim("name", "whatever") }
                        );
                }

            });
        }
    }
}
