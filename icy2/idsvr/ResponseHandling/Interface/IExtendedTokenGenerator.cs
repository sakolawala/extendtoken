using icy2.idsvr.ResponseHandling.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace icy2.idsvr.ResponseHandling.Interface
{
    public interface IExtendedTokenGenerator
    {
        Task<string> ProcessAsync(ExtendTokenRequest extendTokenRequest);
    }
}