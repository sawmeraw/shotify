using System;
using Shotify.Models;

namespace Shotify.Data;

public interface IBrandImageUrlParamRepository
{
    public List<BrandImageUrlParam> GetParams(int brandId);
}
