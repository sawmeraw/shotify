using System;
using System.ComponentModel.DataAnnotations;

namespace Shotify.Models;

public class BrandImageUrl
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Order must be at least 1")]
    public int Order { get; set; }
    // https:...//[itemCode] so we can replace this with the params
    [Url(ErrorMessage = "Provide a valid url.")]
    public string Pattern { get; set; }

}
