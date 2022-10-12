using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.ResponseType = builder.Configuration["Authentication:Cognito:ResponseType"];
    options.MetadataAddress = builder.Configuration["Authentication:Cognito:MetadataAddress"];
    options.ClientId = builder.Configuration["Authentication:Cognito:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Cognito:ClientSecret"];
    options.SaveTokens = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "email",
    };

    options.Events = new OpenIdConnectEvents()
    {
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            context.ProtocolMessage.IssuerAddress = builder.Configuration["Authentication:Cognito:EndSessionEndPoint"];
            context.ProtocolMessage.SetParameter("client_id", builder.Configuration["Authentication:Cognito:ClientId"]);
            context.ProtocolMessage.SetParameter("redirect_uri", $"{context.Request.Scheme}://{context.Request.Host}");
            context.ProtocolMessage.SetParameter("response_type", builder.Configuration["Authentication:Cognito:ResponseType"]);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
