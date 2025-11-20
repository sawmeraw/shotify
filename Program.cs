using System.Data;
using Microsoft.Data.Sqlite;
using Shotify.Data;
using Shotify.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
