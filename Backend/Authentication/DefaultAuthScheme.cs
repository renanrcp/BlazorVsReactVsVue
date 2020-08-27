using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Authentication
{
    public class DefaultAuthScheme : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string HEADER_AUTH_NAME = "Authorization";

        public const string SCHEME_NAME = "Default";

        public DefaultAuthScheme(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var cache = Context.RequestServices.GetRequiredService<IDistributedCache>();

            if (!Request.Headers.ContainsKey(HEADER_AUTH_NAME))
                return AuthenticateResult.Fail("No authorization header.");

            var authorization = Request.Headers[HEADER_AUTH_NAME];

            var userBytes = await cache.GetAsync($"tokens_{authorization}");

            if (userBytes == null || userBytes.Length <= 0)
                return AuthenticateResult.Fail("Unauthorized.");

            var user = JsonSerializer.Deserialize<User>(userBytes);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, user.Name, ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String),
            };

            var identity = new ClaimsIdentity(claims, SCHEME_NAME);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SCHEME_NAME);

            return AuthenticateResult.Success(ticket);
        }
    }
}