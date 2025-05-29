namespace repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;

public class RespondAPI<T> where T : class
{
    public ResultRespond Result { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
        
    public RespondAPI()
    {

    }
        
    public RespondAPI(ResultRespond result, string message, T data = null)
    {
        Result = result;
        Message = message;
        Data = data;
    }
}

public enum ResultRespond
{
    Error, Succeeded, Failed, NotFound, Duplication, UnApproved, NeedChangePassword
}