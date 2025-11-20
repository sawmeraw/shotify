using System;
using System.Data;
using Dapper;
using Shotify.Data;
using Shotify.Models;

namespace Shotify.Services;

public class URLService : IURLService
{

    private readonly IBrandRepository _brandRepo;
    private readonly IBrandImageUrlRepository _brandUrlRepo;
    private readonly IBrandImageUrlParamRepository _brandUrlParamRepo;

    public URLService(IBrandRepository brandRepo, IBrandImageUrlParamRepository brandUrlParamRepo, IBrandImageUrlRepository brandUrlRepo)
    {
        _brandRepo = brandRepo;
        _brandUrlRepo = brandUrlRepo;
        _brandUrlParamRepo = brandUrlParamRepo;
    }

    public List<string>? GenerateImageUrls(int brandId, string payload)
    {
        var brand = _brandRepo.GetBrandById(brandId);

        string[] parts;

        if (!string.IsNullOrEmpty(brand.ProductCodeDelimiterChar))
        {
            parts = payload.Split(brand.ProductCodeDelimiterChar);
        }
        else
        {
            parts = [payload];
        }

        var dict = new Dictionary<string, string>();
        dict["itemCode"] = parts.Length >= 1 ? parts[0] : payload;

        if (parts.Length >= 2)
        {
            dict["colorCode"] = parts[1];

            foreach (var p in _brandUrlParamRepo.GetParams(brandId))
            {
                if (!string.IsNullOrWhiteSpace(p.FixedValue) && p.IsKeepInUrl)
                {
                    var key = p.PlaceholderInUrl.Trim('[', ']');
                    dict[key] = p.FixedValue;
                }
            }
        }

        var urlPatterns = _brandUrlRepo.GetPatterns(brandId);
        if (urlPatterns == null)
        {
            return null;
        }

        var results = urlPatterns
        .Select(u =>
        {
            var url = u.Trim();
            foreach (var kvp in dict)
            {
                url = url.Replace($"[{kvp.Key}]", kvp.Value);
            }
            return url;
        })
        .ToList();

        return results;

    }
}
