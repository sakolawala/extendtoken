using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.Endpoints.Results
{
    public class CustomGrantValidationResult : ValidationResult
    {
        public string GrantType { get; set; }
    }
}
