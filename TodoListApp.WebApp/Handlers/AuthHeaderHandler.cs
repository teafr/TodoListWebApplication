using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserManager<IdentityUser> userManager;
    private readonly ITokenGenerator tokenGenerator;

    public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, ITokenGenerator tokenGenerator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.userManager = userManager;
        this.tokenGenerator = tokenGenerator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ExceptionHelper.CheckObjectForNull(request);

        var context = this.httpContextAccessor.HttpContext;
        var token = context?.Request.Cookies["JWToken"];

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(5) && context is not null)
                {
                    var user = await this.userManager.GetUserAsync(context.User);
                    if (user is not null)
                    {
                        var newToken = this.tokenGenerator.GenerateToken(user);

                        context.Response.Cookies.Append("JWToken", newToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(7),
                        });

                        token = newToken;
                        jwtToken = handler.ReadJwtToken(newToken);
                    }
                }

                if (jwtToken.ValidTo > DateTime.UtcNow)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    context?.Response.Cookies.Delete("JWToken");
                }
            }
            catch (SecurityTokenException)
            {
                context?.Response.Cookies.Delete("JWToken");
            }
            catch (Exception)
            {
                context?.Response.Cookies.Delete("JWToken");
                throw;
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
