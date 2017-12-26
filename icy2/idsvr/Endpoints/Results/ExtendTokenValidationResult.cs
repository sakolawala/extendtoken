using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.Endpoints.Results
{
    public class ExtendTokenValidationResult
    {
        public bool IsError { get; set; }
        public string Error { get; set; }
        public string JWTTokenRAW { get; set; }
    }
}
