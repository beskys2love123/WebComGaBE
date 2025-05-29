using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;

namespace repo_nha_hang_com_ga_BE.Models.Common.Respond;

public class RespondAPIPaging<T> where T : class
{
    public ResultRespond Result { get; set; }
    public string Code { get; set; }
    public PagingResponse<T> Data { get; set; }
    public string Message { get; set; }

    public RespondAPIPaging(ResultRespond result, string message = "", PagingResponse<T> data = null)
    {
        Result = result;
        Data = data;
        Message = message;
    }

    public RespondAPIPaging()
    {
    }
}