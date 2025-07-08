namespace TodoListApp.WebApp.Models;

public class PaginationInfo
{
    public int CountOfItems { get; set; }

    public int PageSize { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)this.CountOfItems / this.PageSize);
}
