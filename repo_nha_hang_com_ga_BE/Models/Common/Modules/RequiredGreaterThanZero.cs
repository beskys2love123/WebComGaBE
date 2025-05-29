using System.ComponentModel.DataAnnotations;

namespace repo_nha_hang_com_ga_BE.Models.Common.Modules;

public class RequiredGreaterThanZero : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        double i;
        return value != null && double.TryParse(value.ToString(), out i) && i > 0;
    }
}