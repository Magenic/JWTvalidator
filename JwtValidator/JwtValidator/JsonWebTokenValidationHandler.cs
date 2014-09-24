using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JwtValidator
{
    public class JsonWebTokenValidationHandler : DelegatingHandler
    {
        private const string JWT_ENABLED_KEY = "JWTEnabled_Key";
        private readonly string _masterKey;

        public JsonWebTokenValidationHandler(string masterKey)
        {
            _masterKey = masterKey;
        }

        public static void Register(HttpConfiguration config, string masterKey)
        {
            // guard double register
            object jwtEnabled;
            if (config.Properties.TryGetValue(JWT_ENABLED_KEY, out jwtEnabled))
                return;

            var existingInitializer = config.Initializer;
            config.Initializer = (httpConfig) =>
            {
                // chain in the existing initializer first
                existingInitializer(httpConfig);

                object innerJwtEnabled;
                if (config.Properties.TryGetValue(JWT_ENABLED_KEY, out innerJwtEnabled))
                    return;

                httpConfig.MessageHandlers.Add(new JsonWebTokenValidationHandler(masterKey));
                httpConfig.Properties[JWT_ENABLED_KEY] = true;
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // browser sends OPTIONS requests to check CORS and will not include Authorization header
            // allow these requests to flow through with the response from the CORS handler
            if (request.Method != HttpMethod.Options)
            {
                string token;
                if (TryRetrieveToken(request, out token))
                {
                    try
                    {
                        var jwt = new JsonWebToken(token, new Dictionary<int, string> {{0, _masterKey}});
                        jwt.Validate(validateExpiration: true);

                        SetCurrentPrincipal(new JwtPrincipal(new JwtIdentity(jwt.Claims)));
                    }
                    catch (JsonWebTokenException)
                    {
                    }
                }
            }
            return base.SendAsync(request, cancellationToken);
        }

        private void SetCurrentPrincipal(IPrincipal principal)
        {
          System.Threading.Thread.CurrentPrincipal = principal;
        }

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeadersEnum;
            bool hasHeader = request.Headers.TryGetValues("Authorization", out authzHeadersEnum);
            if (!hasHeader)
                return false;

            var authzHeaders = authzHeadersEnum.ToList();
            if (authzHeaders.Count > 1)
                return false;

            // Remove the bearer token scheme prefix and return the rest as ACS token  
            var bearerToken = authzHeaders[0];
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }
    }
}