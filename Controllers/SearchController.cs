using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            try
            {
                var urls = _urlService.GenerateImageUrls(5, "AL0A85Q1001");
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
