using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using ShopTFTEC.Admin.Configuration;
using ShopTFTEC.Admin.Context;
using ShopTFTEC.Admin.Models;
using ShopTFTEC.Admin.Policies;
using ShopTFTEC.Admin.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connection));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

//configurações dos serviços do IdentityServer
var builderIdentityServer = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;

}).AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
                       .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                       .AddInMemoryClients(IdentityConfiguration.Clients)
                       .AddAspNetIdentity<ApplicationUser>();


builder.Services.Configure<ConfigurationImagens>(options =>
{
    options.NomePastaImagensProdutos = builder.Configuration["ConfigurationPastaImagens:NomePastaImagensProdutos"];
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", c =>
    {
        c.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        c.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToAccessDenied = (context) =>
            {
                context.HttpContext.Response.Redirect(builder.Configuration["ServiceUri:IdentityServer"] + "/Account/AccessDenied");
                return Task.CompletedTask;
            }
        };
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/");
            context.HandleResponse();

            return Task.FromResult(0);
        };

        options.Authority = builder.Configuration["ServiceUri:IdentityServer"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = "identity";
        options.ClientSecret = builder.Configuration["Client:Secret"];
        options.ResponseType = "code";
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.ClaimActions.MapJsonKey("sub", "sub", "sub");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add("identity");
        options.SaveTokens = true;
    }
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserAdminGerenteRole",
         policy => policy.RequireRole("User", "Admin", "Gerente"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdminClaimAccess",
        policy => policy.RequireClaim("CadastradoEm"));

    options.AddPolicy("IsAdminClaimAccess",
        policy => policy.RequireClaim("IsAdmin", "true"));

    options.AddPolicy("IsFuncionarioClaimAccess",
       policy => policy.RequireClaim("IsFuncionario", "true"));

    options.AddPolicy("TempoCadastroMinimo", policy =>
    {
        policy.Requirements.Add(new TempoCadastroRequirement(5));
    });

    //options.AddPolicy("TesteClaim",
    //policy => policy.RequireClaim("Teste", "teste_claim"));
});

builder.Services.AddHttpClient<IProductService, ProductService>("ProductApi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:ProductApi"]);
    c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
    c.DefaultRequestHeaders.Add("Keep-Alive", "3600");
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-ProductApi");
});

builder.Services.AddHttpClient<ICategoryService, CategoryService>("CategoryApi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:ProductApi"]);
    c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
    c.DefaultRequestHeaders.Add("Keep-Alive", "3600");
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-ProductApi");
});

builder.Services.AddScoped<IAuthorizationHandler, TempoCadastroHandler>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<ISeedUserClaimsInitial, SeedUserClaimsInitial>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProfileService, ProfileAppService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

IdentityModelEventSource.ShowPII = true;

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

await CriarPerfisUsuariosAsync(app);

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MinhaArea",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory?.CreateScope())
    {
        var service = scope?.ServiceProvider.GetService<ISeedUserRoleInitial>();
        await service.SeedRolesAsync();
        await service.SeedUsersAsync();

        var service2 = scope?.ServiceProvider.GetService<ISeedUserClaimsInitial>();
        await service2.SeedUserClaims();
    }
}
