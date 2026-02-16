using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shotify.Data;
using Shotify.Models;

namespace Shotify.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IBrandRepository _repo;

    public HomeController(IBrandRepository repo)
    {
        _repo = repo;
    }
    [Route("/", Name = "HomePage")]
    public IActionResult Index()
    {
        var brandList = _repo.GetBrandList().Where(b => !b.IsDeleted).ToList();
        return View(brandList);
    }

    [Route("/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/editor")]
    public IActionResult Editor()
    {
        return View();
    }

    [Route("/parser")]
    public IActionResult ExcelParser()
    {
        return View();
    }

    [Route("/about")]
    public IActionResult About()
    {
        return View();
    }

    [HttpPost]
    [Route("/api/parser")]
    public async Task<IActionResult> ProcessExcel(IFormFile file, [FromForm] string brand)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });
        if (string.IsNullOrEmpty(brand))
            return BadRequest(new { message = "Brand is required." });

        using var httpClient = new HttpClient();
        using var content = new MultipartFormDataContent();
        using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        content.Add(fileContent, "file", file.FileName);
        content.Add(new StringContent(brand), "brand");

        var goApiUrl = Environment.GetEnvironmentVariable("GO_API_URL") ?? "http://localhost:8080";
        var response = await httpClient.PostAsync($"{goApiUrl}/process", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new { message = error });
        }

        var resultStream = await response.Content.ReadAsStreamAsync();
        return File(resultStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rex_output.xlsx");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
