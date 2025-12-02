using System;
using Microsoft.Extensions.Primitives;
using Models.DTOs;

namespace Shotify.Models.DTOs;
//UpdateBrandDTO is used for validating forms. Thats why it has the other properties in it.

public class UpdateBrandDTO : CreateBrandDTO
{
    public int Id { get; set; }
    public List<BrandImageUrlDTO>? ImageUrls { get; set; }
    public List<BrandImageUrlParam>? UrlParams { get; set; }
    public List<CreateBrandImageUrlDTO>? NewUrls { get; set; }
    public List<CreateBrandImageUrlParamDTO>? NewParams { get; set; }

}
