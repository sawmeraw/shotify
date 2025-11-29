using Microsoft.AspNetCore.Mvc;
using Shotify.Data;
using ViewModels;
using Shotify.Models.DTOs;


namespace Shotify.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandRepository _repo;
        public BrandController(IBrandRepository repo)
        {
            _repo = repo;
        }

        [Route("/admin/brand/{id}")]
        [HttpGet]
        public IActionResult Index(int id)
        {
            var brand = _repo.GetBrandById(id);
            var viewModel = new EditBrandViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                ProductCodeCutOffChar = brand.ProductCodeCutOffChar,
                ProductCodeDelimiterChar = brand.ProductCodeDelimiterChar,
                ProductCodeDelimiterOffset = brand.ProductCodeDelimiterOffset,
                ProductCodeSliceOffset = brand.ProductCodeSliceOffset
            };

            return View(viewModel);
        }

        [Route("/admin/brand/{id}")]
        [HttpPost]
        public IActionResult Edit(int id, UpdateBrandDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            _repo.UpdateBrand(id, dto);
            TempData["Message"] = "Changes saved!";
            return RedirectToAction("Index", new { id });
        }
    }

}
