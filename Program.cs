using System.Data;
using Microsoft.Data.Sqlite;
using Shotify.Data;
using Shotify.Services;

var builder = WebApplication.CreateBuilder(args);
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var key = System.Text.Encoding.ASCII.GetBytes(secretKey);

//add jwt for auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
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

// builder.WebHost.ConfigureKestrel(options =>
//     {
//         options.ListenLocalhost(5012);
//         options.ListenLocalhost(7012, listenOptions =>
//         {
//             listenOptions.UseHttps();
//         });
// });

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

// app.UseMiddleware<LoginMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllers();

app.Run();