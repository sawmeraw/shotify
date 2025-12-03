using Microsoft.AspNetCore.Mvc;
using Shotify.Data;
using ViewModels;
using Shotify.Models.DTOs;
using Shotify.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;


namespace Shotify.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandRepository _brandRepo;
        private readonly IBrandImageUrlRepository _urlRepo;
        private readonly IBrandImageUrlParamRepository _paramRepo;
        public BrandController(IBrandRepository brandRepo, IBrandImageUrlRepository urlRepo, IBrandImageUrlParamRepository paramRepo)
        {
            _brandRepo = brandRepo;
            _urlRepo = urlRepo;
            _paramRepo = paramRepo;
        }

        [Route("/admin/brands")]
        [HttpGet]
        public IActionResult List(int id)
        {
            return View();
        }

        [Route("/admin/brand/{id}")]
        [HttpGet]
        public IActionResult Index(int id)
        {
            var brand = _brandRepo.GetBrandById(id);
            var brandImageUrls = _urlRepo.GetBrandImageUrlParams(id);
            var brandUrlParms = _paramRepo.GetParams(id, false);
            var viewModel = new EditBrandViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                ProductCodeCutOffChar = brand.ProductCodeCutOffChar,
                ProductCodeDelimiterChar = brand.ProductCodeDelimiterChar,
                ProductCodeDelimiterOffset = brand.ProductCodeDelimiterOffset,
                ProductCodeSliceOffset = brand.ProductCodeSliceOffset,
            };

            if (brandImageUrls != null)
            {
                viewModel.ImageUrls = brandImageUrls;
            }

            if (brandUrlParms != null)
            {
                viewModel.UrlParams = brandUrlParms;
            }

            return View(viewModel);
        }

        [Route("/admin/brand/{id}")]
        [HttpPost]
        public IActionResult Save(int id, UpdateBrandDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _brandRepo.UpdateBrand(id, new UpdateBrandDTO
            {
                Id = id,
                Name = dto.Name,
                ProductCodeCutOffChar = dto.ProductCodeCutOffChar,
                ProductCodeDelimiterChar = dto.ProductCodeDelimiterChar,
                ProductCodeDelimiterOffset = dto.ProductCodeDelimiterOffset,
                ProductCodeSliceOffset = dto.ProductCodeSliceOffset,
            });

            if (dto.ImageUrls != null)
            {
                _urlRepo.UpdateBrandImageUrls(dto.ImageUrls);
            }

            if (dto.UrlParams != null)
            {
                _paramRepo.UpdateParams(dto.UrlParams);
            }

            if (dto.NewUrls != null && dto.NewUrls.Count != 0)
            {
                _urlRepo.CreateBrandImageUrls(dto.NewUrls);
            }

            if (dto.NewParams != null && dto.NewParams.Count != 0)
            {
                _paramRepo.CreateParams(dto.NewParams);
            }

            TempData["Message"] = "Changes saved!";
            return RedirectToAction("Index", new { id });
        }
    }

}
