using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        // this is the extension method that we created to get the username and ID from the token 
        // how it work is that we are going to pass in the ClaimsPrincipal user and we are going to look for the ClaimTypes.NameIdentifier
        // and we are going to return the value of that claim
        // this is the ClaimTypes.NameIdentifier that we created in the TokenService.cs
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}