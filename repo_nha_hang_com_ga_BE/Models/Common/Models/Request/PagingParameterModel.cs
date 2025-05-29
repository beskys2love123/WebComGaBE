using System.ComponentModel.DataAnnotations;
using repo_nha_hang_com_ga_BE.Models.Common.Modules;

namespace repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

public class PagingParameterModel
{
    const int MaxPageSize = 100;

    public bool IsPaging { get; set; } = true;

    [RequiredGreaterThanZero]

    public int PageNumber { get; set; } = 1;

    private int _pageSize { get; set; } = 20;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}