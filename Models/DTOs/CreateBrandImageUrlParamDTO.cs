using System;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class CreateBrandImageUrlParamDTO
{
    public int BrandId { get; set; }
    //itemCode
    public string Name { get; set; }
    //for ui 
    public string Description { get; set; }
    //keep track of placeholders
    public int Order { get; set; }
    //if it has default value that can be set like version in Hoka urls
    public string? FixedValue { get; set; }
    //To parse it in url by default as fixed value
    public bool IsKeepInUrl { get; set; }
    public bool IsAllLowerCase { get; set; }
    public bool IsAllUpperCase { get; set; }
    //to define something like [itemCode]
    public string PlaceholderInUrl { get; set; }

}

