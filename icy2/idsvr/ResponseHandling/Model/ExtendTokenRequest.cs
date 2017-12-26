using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr.ResponseHandling.Model
{
    public class ExtendTokenRequest
    {
        public string JWTTokenRAW { get; set; }
        public List<string> AudiencesToAdd { get; set; }
    }
}
