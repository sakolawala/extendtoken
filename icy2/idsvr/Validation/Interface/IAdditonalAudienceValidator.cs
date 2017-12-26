using System.Threading.Tasks;
using icy2.idsvr.Endpoints.Results;
using Microsoft.AspNetCore.Http;

namespace icy2.idsvr.Validation.Interface
{
    public interface IAdditonalAudienceValidator
    {
        Task<AdditonalAudienceValidationResult> ValidateAsync(HttpContext context);
    }
}