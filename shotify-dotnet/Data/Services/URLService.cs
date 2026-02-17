using System;
using System.Data;
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

        if (!string.IsNullOrEmpty(brand.ProductCodeCutOffChar))
        {
            var charIndex = payload.IndexOf(brand.ProductCodeCutOffChar);
            if (charIndex == -1)
            {
                throw new Exception("Cut off character not found in the product code");
            }
            var strippedCode = payload.Substring(0, charIndex);
            parts = [strippedCode];
        }

        if (brand.ProductCodeDelimiterOffset != null)
        {
            parts = [payload.Substring(0, brand.ProductCodeDelimiterOffset.Value), payload.Substring(brand.ProductCodeDelimiterOffset.Value)];
        }

        var dict = new Dictionary<string, string?>();

        var currParams = _brandUrlParamRepo.GetParams(brandId, true);

        if (currParams == null)
        {
            throw new Exception("No parameters found on the URL to replace.");
        }

        int expectedPartsCount = currParams.Count(p=> p.FixedValue == null || !p.IsKeepInUrl);
        if (parts.Length < expectedPartsCount)
        {
            throw new Exception($"Not enough parts in the payload to replace all parameters. Expected at least {expectedPartsCount} parts, but got {parts.Length}.");
        }

        //keep track of how many parts have been used for params, so the final else if condition doesnt go out of bounds especially for Hoka URLs
        int partIndex = 0;
        for (int i = 0; i < currParams.Count; i++)
        {
            var param = currParams[i];
            string key = param.PlaceholderInUrl.Trim('[', ']');

            string value = null;
            // Console.WriteLine($"current param order: {param.Order}, value: {param.PlaceholderInUrl}, curr i: {i}, curr parts.Length : {parts.Length}");
            if (param.PlaceholderInUrl == "[productCode]" && parts.Length == 1)
            {
                value = parts[0];
            }
            else if (param.PlaceholderInUrl == "[itemCode]")
            {
                partIndex++;
                value = parts.Length > 1 ? parts[0] : payload;
            }
            else if (!string.IsNullOrEmpty(param.FixedValue) && param.IsKeepInUrl)
            {
                value = param.FixedValue;
            }
            else if (i < currParams.Count)
            {
                value = parts[partIndex];
                // Console.WriteLine($"Value here: {value}");
            }

            if (value != null)
            {
                if (param.IsAllLowerCase)
                {
                    value = value.ToLowerInvariant();
                }
                if (param.IsAllUpperCase)
                {
                    value = value.ToUpperInvariant();
                }
            }

            dict[key] = value;
        }

        var urlPatterns = _brandUrlRepo.GetPatterns(brandId);
        if (urlPatterns == null)
        {
            throw new Exception("No Url exceptions found in the product code");
        }

        var results = urlPatterns
        .Select(u =>
        {
            var url = u.Trim();
            foreach (var kvp in dict)
            {
                // Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                url = url.Replace($"[{kvp.Key}]", kvp.Value);
            }
            return url;
        })
        .ToList();

        return results;

    }
}
