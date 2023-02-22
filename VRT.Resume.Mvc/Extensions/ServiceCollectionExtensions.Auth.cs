using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc;

internal static partial class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddCookieAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication(options =>
         {
             options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
         })
            .AddCookie(options =>
            {
                // Cookie settings      
                options.Cookie.Name = Globals.AuthCookieName;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/Account/Index");
            });
    }
    public static AuthenticationBuilder AddAuthenticationProviders(this AuthenticationBuilder authBuilder,
        IConfiguration config)
    {
        var auth = config?.GetSection("Auth")?.Get<AuthConfigSection>();
        if (null == auth || auth.Providers == null || auth.Providers.Length == 0)
            return authBuilder;

        foreach (var p in auth.Providers)
        {
            var _ = AddGoogleProvider(authBuilder, p) || AddGithubProvider(authBuilder, p);
        }
        return authBuilder;
    }

    private static bool AddGoogleProvider(AuthenticationBuilder authBuilder,
        AuthConfigSection.AuthProvider provider)
    {
        if (provider?.Name?.ToLower() != "google")
            return false;

        authBuilder.AddGoogle(options =>
        {
            options.ClientId = provider.ClientId;
            options.ClientSecret = provider.ClientSecret;
            options.CallbackPath = new PathString(provider.CallbackPath ?? $"/signin-{provider.Name}");
        });
        return true;
    }

    private static bool AddGithubProvider(AuthenticationBuilder authBuilder,
        AuthConfigSection.AuthProvider provider)
    {
        if (provider?.Name?.ToLower() != "github")
            return false;
        authBuilder.AddOAuth(provider.Name, options =>
         {
             options.ClientId = provider.ClientId;
             options.ClientSecret = provider.ClientSecret;
             options.CallbackPath = new PathString(provider.CallbackPath);
             options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
             options.TokenEndpoint = "https://github.com/login/oauth/access_token";
             options.UserInformationEndpoint = "https://api.github.com/user";
             options.Scope.Add("user:email");
             options.SaveTokens = true;
             options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
             options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
             options.Events = new OAuthEvents
             {
                 OnCreatingTicket = async context =>
                 {
                     var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                     request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                     var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                     response.EnsureSuccessStatusCode();
                     var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                     context.RunClaimActions(json.RootElement);
                 }
             };
         });
        return true;
    }
}
