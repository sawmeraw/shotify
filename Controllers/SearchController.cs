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
                return StatusCode(StatusCodes.Status500InternalServerError, "Oops! Something went wrong.");
            }
        }

    }
}
