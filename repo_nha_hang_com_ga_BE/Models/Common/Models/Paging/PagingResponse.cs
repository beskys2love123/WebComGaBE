namespace repo_nha_hang_com_ga_BE.Models.Common.Paging;

public class PagingResponse<T> where T : class
{
    public PagingDetail Paging { get; set; }
    public T Data { get; set; }
}