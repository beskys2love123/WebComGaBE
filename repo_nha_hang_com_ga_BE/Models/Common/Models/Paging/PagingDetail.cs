namespace repo_nha_hang_com_ga_BE.Models.Common.Paging;

public class PagingDetail
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public long TotalRecords { get; set; }

    public PagingDetail() { }

    public PagingDetail(int currentPage, int pageSize, long totalRecords)
    {
        this.CurrentPage = currentPage;
        this.PageSize = pageSize;
        this.TotalRecords = totalRecords;
    }
}