using System;
using Models.DTOs;

namespace Shotify.Services;

public interface IBrandImageUrlRepository
{
    public List<string>? GetPatterns(int brandId);
    public List<BrandImageUrlReadDTO>? GetBrandImageUrls(int brandId);
    public void UpdateBrandImageUrls(List<BrandImageUrlReadDTO> items);
    public void CreateBrandImageUrls(List<CreateBrandImageUrlDTO> items);
}
