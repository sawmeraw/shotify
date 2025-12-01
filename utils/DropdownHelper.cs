using Microsoft.AspNetCore.Mvc.Rendering;

public static class DropdownHelper
{
    public static SelectList BooleanSelectList(bool? selectedValue = null)
    {
        var items = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "True" },
            new SelectListItem { Value = "false", Text = "False" }
        };

        return new SelectList(items, "Value", "Text", selectedValue?.ToString().ToLower());
    }
}