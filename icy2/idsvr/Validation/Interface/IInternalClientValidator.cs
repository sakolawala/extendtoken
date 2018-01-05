using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation.Interface
{
    public interface IInternalClientValidator
    {
        //
        // Summary:
        //     Tries to authenticate a client based on the incoming request
        //
        // Parameters:
        //   context:
        //     The context.
        Task<ClientSecretValidationResult> ValidateAsync(HttpContext context);
    }
}
