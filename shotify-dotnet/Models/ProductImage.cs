using System;
using System.Text.Json.Serialization;

namespace Shotify.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    public string ItemCode { get; set; }
    public string? ColorCode { get; set; }
    public string ImageUrl { get; set; }
    public DateTime FetchedAt { get; set; }

    [JsonIgnore]
    public Brand? Brand { get; set; }
}
