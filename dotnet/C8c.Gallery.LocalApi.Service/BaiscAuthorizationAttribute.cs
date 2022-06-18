using Microsoft.AspNetCore.Authorization;

namespace C8c.Gallery.LocalApi.Service
{
    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
