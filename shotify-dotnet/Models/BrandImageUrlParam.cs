using System;
using System.ComponentModel.DataAnnotations;

namespace Shotify.Models;

public class BrandImageUrlParam
{
    public int Id { get; set; }
    public int BrandId { get; set; }
    //itemCode
    [Required(ErrorMessage ="Name is required.")]
    public string Name { get; set; }
    //for ui 
    public string? Description { get; set; }
    //keep track of placeholders
    public int? Order { get; set; }
    //if it has default value that can be set like version in Hoka urls
    public string? FixedValue { get; set; }
    //To parse it in url by default as fixed value
    public bool IsKeepInUrl { get; set; }
    public bool IsAllLowerCase { get; set; }
    public bool IsAllUpperCase { get; set; }
    //to define something like [itemCode]
    [Required(ErrorMessage ="Required")]
    public string PlaceholderInUrl { get; set; }

}

