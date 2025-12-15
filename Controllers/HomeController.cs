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
        var brandList = _repo.GetBrandList();
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
