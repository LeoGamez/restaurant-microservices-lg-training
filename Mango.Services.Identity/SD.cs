using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Services.Identity
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>()
            {
                new ApiScope("mango","Mango Server"),
                new ApiScope(name:"read",displayName: "Read Your Data"),
                new ApiScope(name:"write",displayName: "Write Your Data"),
                new ApiScope(name:"delete",displayName: "Delete Your Data")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            { 
                
                new Client
                {
                    ClientId="mango",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        "https://localhost:7161/signin-oidc"
                    },
                    PostLogoutRedirectUris = {
                         "https://localhost:7161/signout-callback-oidc"
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "mango"
                    }
                }
            };
    }
}