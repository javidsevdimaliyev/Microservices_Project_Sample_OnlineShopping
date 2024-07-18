using System.Security.Claims;

namespace BasketService.Api.Core.Application.Services;

public static class IdentityHelper 
{
    public static long UserId => GetUserId();
    public static string? UserName => GetUserName();
    public static ClaimsIdentity UserIdentity => GetUserIdentity();
    public static bool UserIsAuthenticated => GetUserIsAuthenticated();


    public static string GetUserName()
    {
        return new HttpContextAccessor().HttpContext.User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier).Value;
    }

 
    private static ClaimsIdentity GetUserIdentity()
    {
        return (ClaimsIdentity)new HttpContextAccessor().HttpContext?.User?.Identity;
    }

    public static int GetId()
    {
        var userId = GetUserIdentity().FindFirst("UserId")?.Value;
        return !string.IsNullOrEmpty(userId) ? Convert.ToInt32(userId) : 0;
    }

    public static int GetUserId()
    {
        var userId = GetUserIdentity()?.FindFirst("UserId")?.Value;
        //var result = Convert.ToInt32(GetUserIdentity().FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return !string.IsNullOrEmpty(userId) ? Convert.ToInt32(userId) : 0;
    }

     
    private static bool GetUserIsAuthenticated()
    {
        return GetUserIdentity().IsAuthenticated;
    }

  
}
