using System;
using Models.DTOs;
using Shotify.Models;

namespace Shotify.Data;

public interface IBrandImageUrlParamRepository
{
    public List<BrandImageUrlParam> GetParams(int brandId, bool sortByFixedValue);
    public void UpdateParams(List<BrandImageUrlParam> items);
    public void CreateParams(List<CreateBrandImageUrlParamDTO> items);
}
