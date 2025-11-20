using System;

namespace Shotify.Services;

public interface IBrandImageUrlRepository
{
    public List<string>? GetPatterns(int brandId);
}
