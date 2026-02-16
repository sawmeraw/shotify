using System;
using Models.DTOs;
using Shotify.Data;

namespace Shotify.Services;

public class BrandImageUrlRepository : IBrandImageUrlRepository
{
    private readonly AppDbContext _db;

    public BrandImageUrlRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<string>? GetPatterns(int brandId)
    {
        var patterns = _db.BrandImageUrls
            .Where(u => u.BrandId == brandId)
            .OrderBy(u => u.Order)
            .Select(u => u.Pattern)
            .ToList();

        return patterns.Count == 0 ? null : patterns;
    }

    public List<BrandImageUrlReadDTO>? GetBrandImageUrls(int brandId)
    {
        var patterns = _db.BrandImageUrls
            .Where(u => u.BrandId == brandId)
            .OrderBy(u => u.Order)
            .Select(u => new BrandImageUrlReadDTO
            {
                Id = u.Id,
                Order = u.Order,
                Pattern = u.Pattern
            })
            .ToList();

        return patterns.Count == 0 ? null : patterns;
    }

    public void UpdateBrandImageUrls(List<BrandImageUrlReadDTO> items)
    {
        try
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                var entity = _db.BrandImageUrls.FirstOrDefault(u => u.Id == item.Id);
                if (entity != null)
                {
                    entity.Order = item.Order ?? entity.Order;
                    entity.Pattern = item.Pattern;
                }
            }
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlRepository.UpdateBrandImageUrls: {e.Message}");
            throw new Exception("Error updating brand image urls");
        }
    }

    public void CreateBrandImageUrls(List<CreateBrandImageUrlDTO> items)
    {
        try
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Order = i + 1;
            }

            var entities = items.Select(item => new Shotify.Models.BrandImageUrl
            {
                BrandId = item.BrandId,
                Order = item.Order ?? 0,
                Pattern = item.Pattern
            }).ToList();

            _db.BrandImageUrls.AddRange(entities);
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlRepository.CreateBrandImageUrls: {e.Message}");
            throw new Exception("Error occurred while creating image urls");
        }
    }
}
