using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace icy2.idsvr
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
                            {
                                new ApiResource("api1", "My API"),
                                new ApiResource("api2", "Cool API"),
                                new ApiResource("api3", "Super Cool API")
                            };
        }

        public static IEnumerable<Client> GetClients() => new List<Client>
            {
                    new Client
                    {
                        ClientId = "client",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        // scopes that client has access to
                        AllowedScopes = { "api1" },
                        IdentityTokenLifetime = 3600
                    },
                    new Client
                    {
                        ClientId = "InternalServiceClient",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }                        
                    },
                    new Client
                    {
                        ClientId = "ro.client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = { "api1" },
                        AllowOfflineAccess = true,
                        IdentityTokenLifetime = 3600
                    }
            };
    }
}
