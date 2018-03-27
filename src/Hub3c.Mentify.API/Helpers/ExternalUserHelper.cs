using System;
using System.Linq;
using System.Security.Claims;

namespace Hub3c.Mentify.API.Helpers
{
    public static class ExternalUserHelper
    {
        public static int GetMid(this ClaimsPrincipal user)
        {
            if (user.Claims.Any(e => e.Type == "mid"))
            {
                var mid = user.Claims.FirstOrDefault(a => a.Type == "mid");
                if (!string.IsNullOrEmpty(mid?.Value))
                    return int.Parse(mid.Value);
            }
            throw new ApplicationException("The request does not pass the mid value");
        }
    }
}