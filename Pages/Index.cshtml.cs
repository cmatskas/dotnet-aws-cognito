using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnet_aws_cognito.Pages;

//[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private IConfiguration _configuration;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void OnGet()
    {

    }

    public void OnPost()
    {
        //get access token from authenticated user
        if(HttpContext.User?.Identity?.IsAuthenticated ?? false)
        {
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;
        }
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    }
}
