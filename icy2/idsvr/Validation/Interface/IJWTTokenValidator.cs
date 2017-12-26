using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation.Interface
{
    public interface IJWTTokenValidator
    {
        Task<TokenValidationResult> ValidateAsync(HttpContext context);
    }
}
