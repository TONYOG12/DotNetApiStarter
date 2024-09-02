namespace APP.Utils;

public class Paginateable<T>
{
    public T Data { get; set; }
    public int PageIndex { get; set; }
    public int PageCount { get; set; }
    public int TotalRecordCount { get; set; }
    public int NumberOfPagesToShow { get; set; }
    public int StartPageIndex { get; set; }
    public int StopPageIndex { get; set; }
}