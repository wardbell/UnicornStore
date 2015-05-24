using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace UnicornStore
{
    public static class FakeUserMiddleware
    {
        public static IApplicationBuilder UseFakeUser(this IApplicationBuilder app)
        {
            return app.Use(next => context => SetFakeUser(context, next));
        }

        private static Task SetFakeUser(HttpContext context, RequestDelegate next)
        {
            context.User = new FakeClaimsPrincipal();
            return next(context);
        }

        private class FakeClaimsPrincipal : ClaimsPrincipal
        {
            public FakeClaimsPrincipal() : base(new FakeUser())
            {
                // See https://github.com/aspnet/Identity/blob/dev/src/Microsoft.AspNet.Identity/PrincipalExtensions.cs
                AddIdentity(new ClaimsIdentity(new List<Claim>
                    { new Claim(ClaimTypes.NameIdentifier, "8d00d78b-eaa5-415d-9424-3bfc90a72f54"),
                      new Claim(ClaimTypes.Role, "Admin")}));
            }

            public override bool IsInRole(string role)
            {
                return true;
            }
        }

        private class FakeUser : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "Faked";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "wardbell@gmail.com";
                }
            }
        }
    }
}
