using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.AspNetCore.Builder
{
    public static class YesNoBuilder
    {
        public static void UseSeveralJwtBearer(this IApplicationBuilder app, params string[] schemes)
        {
            app.Use(async (context, next) =>
            {
                var principal = new ClaimsPrincipal();

                foreach(string scheme in schemes)
                {
                    var result1 = await context.AuthenticateAsync(scheme);
                    if (result1?.Principal != null)
                    {
                        principal.AddIdentities(result1.Principal.Identities);
                    }
                }
                context.User = principal;

                await next();
            });
        }
    }
}
