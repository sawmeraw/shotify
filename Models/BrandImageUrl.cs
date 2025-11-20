using System;

namespace Shotify.Models;

public class BrandImageUrl
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public int Order { get; set; }
    // https:...//[itemCode] so we can replace this with the params
    public string Pattern { get; set; }

}
