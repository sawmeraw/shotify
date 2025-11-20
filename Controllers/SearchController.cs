using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shotify.Data;
using Shotify.Services;

namespace Shotify.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IURLService _urlService;
        public SearchController(IURLService urlService)
        {
            _urlService = urlService;
        }
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var urls = _urlService.GenerateImageUrls(1, "1011B157.700");
            if (urls == null)
            {
                return NotFound();
            }
            return Ok(urls);
        }
    }
}
