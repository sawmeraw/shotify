using System.ComponentModel.DataAnnotations;

namespace ViewModels;

public class LoginViewModel
{
    [Required]
    public string Password{get;set;}
    public string? ErrorMessage {get;set;}
}