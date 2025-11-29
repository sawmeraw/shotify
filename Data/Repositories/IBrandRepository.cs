using System;
using Shotify.Models;
using Shotify.Models.DTOs;

namespace Shotify.Data;

public interface IBrandRepository
{
    public Brand GetBrandById(int id);
    public long CreateBrand(CreateBrandDTO payload);

    public void UpdateBrand(int brandId, UpdateBrandDTO payload);
    public List<BrandListItemDTO> GetBrandList();
}
