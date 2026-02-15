using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shotify.Data;
using Shotify.Models.DTOs;
using Shotify.Services;

namespace Shotify.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IURLService _urlService;
        private readonly IBrandRepository _brandRepo;
        public SearchController(IURLService urlService, IBrandRepository brandRepo)
        {
            _urlService = urlService;
            _brandRepo = brandRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetImages([FromQuery] int brandId, [FromQuery] string productCode)
        {
            try
            {
                var brand = _brandRepo.GetBrandById(brandId);
                if (!string.IsNullOrEmpty(brand.ProductCodeDelimiterChar) || !string.IsNullOrEmpty(brand.ProductCodeCutOffChar))
                {
                    bool hasDelimiter = !string.IsNullOrEmpty(brand.ProductCodeDelimiterChar) && productCode.Contains(brand.ProductCodeDelimiterChar);
                    bool hasCutOff = !string.IsNullOrEmpty(brand.ProductCodeCutOffChar) && productCode.Contains(brand.ProductCodeCutOffChar);

                    if (!hasDelimiter && !hasCutOff)
                    {
                        return BadRequest(new ErrorResponse { Message = "Product code doesn't look right for the selected brand." });
                    }
                }
                var urls = _urlService.GenerateImageUrls(brandId, productCode);
                if (urls == null)
                {
                    return NotFound();
                }
                return Ok(urls);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred: {e.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "Oops! We ran into a server error!" });
            }
        }

    }
}
