using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Shotify.Data;
using Shotify.Services;

var builder = WebApplication.CreateBuilder(args);
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");
}

var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

//add jwt for auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            var returnUrl = context.Request.Path + context.Request.QueryString;
            context.Response.Redirect($"/login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IDbConnection>(x =>
{
    var connection = new SqliteConnection("Data Source = shotify.db");
    connection.Open();
    return connection;
});

builder.Services.AddScoped<IBrandImageUrlRepository, BrandImageUrlRepository>();
builder.Services.AddScoped<IBrandImageUrlParamRepository, BrandImageUrlParamRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IURLService, URLService>();

//this is to add views/admin to the view since since there is no admin controller
builder.Services.AddControllersWithViews()
.AddRazorOptions(options =>
{
    options.ViewLocationFormats.Add("Views/Admin/{1}/{0}.cshtml");
    options.PageViewLocationFormats.Add("Pages/{1}/{0}.cshtml");
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles();

//dont change this, it fucks shit up
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllers();

app.Run();