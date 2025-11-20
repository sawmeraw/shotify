using System;

namespace Shotify.Services;

public interface IURLService
{
    public List<string> GenerateImageUrls(int brandId, string payload);
}
