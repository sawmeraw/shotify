using System;
using Models.DTOs;
using Shotify.Models;

namespace Shotify.Data;

public class BrandImageUrlParamRepository : IBrandImageUrlParamRepository
{
    private readonly AppDbContext _db;

    public BrandImageUrlParamRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<BrandImageUrlParam> GetParams(int brandId, bool sortByFixedValue)
    {
        var query = _db.BrandImageUrlParams.Where(p => p.BrandId == brandId);

        if (sortByFixedValue)
        {
            query = query.OrderBy(p =>
                p.FixedValue != null && !p.IsKeepInUrl ? 1 : 0);
        }
        else
        {
            query = query.OrderBy(p => p.Order);
        }

        return query.ToList();
    }

    public void UpdateParams(List<BrandImageUrlParam> items)
    {
        try
        {
            foreach (var item in items)
            {
                var entity = _db.BrandImageUrlParams.FirstOrDefault(p => p.Id == item.Id);
                if (entity != null)
                {
                    entity.Name = item.Name;
                    entity.Description = item.Description;
                    entity.Order = item.Order;
                    entity.FixedValue = item.FixedValue;
                    entity.IsKeepInUrl = item.IsKeepInUrl;
                    entity.IsAllLowerCase = item.IsAllLowerCase;
                    entity.IsAllUpperCase = item.IsAllUpperCase;
                    entity.PlaceholderInUrl = item.PlaceholderInUrl;
                }
            }
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlParamRepository.UpdateParams: {e.Message}");
            throw new Exception("Error updating url params");
        }
    }

    public void CreateParams(List<CreateBrandImageUrlParamDTO> items)
    {
        try
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Order = i;
            }

            var entities = items.Select(item => new BrandImageUrlParam
            {
                BrandId = item.BrandId,
                Name = item.Name,
                Description = item.Description,
                Order = item.Order,
                FixedValue = item.FixedValue,
                IsKeepInUrl = item.IsKeepInUrl,
                IsAllLowerCase = item.IsAllLowerCase,
                IsAllUpperCase = item.IsAllUpperCase,
                PlaceholderInUrl = item.PlaceholderInUrl
            }).ToList();

            _db.BrandImageUrlParams.AddRange(entities);
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught error in BrandImageUrlParamRepository.CreateParams: {e.Message}");
            throw new Exception("Error creating url params");
        }
    }
}
