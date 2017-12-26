using IdentityServer4.Validation;
using System.Collections.Generic;

namespace icy2.idsvr.Endpoints.Results
{
    public class AdditonalAudienceValidationResult : ValidationResult
    {
        public List<string> AdditionAudieces { get; set; }
    }
}
