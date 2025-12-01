using System;
using Models.DTOs;

namespace Shotify.Services;

public interface IBrandImageUrlRepository
{
    public List<string>? GetPatterns(int brandId);
    public List<BrandImageUrlDTO>? GetBrandImageUrlParams(int brandId);
    public void UpdateBrandImageUrls(List<BrandImageUrlDTO> items);
}
