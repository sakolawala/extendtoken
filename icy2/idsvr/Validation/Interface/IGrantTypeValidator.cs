using icy2.idsvr.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace icy2.idsvr.Validation.Interface
{
    public interface IGrantTypeValidator
    {        
        Task<CustomGrantValidationResult> ValidateAsync(HttpContext context);        
    }
}
