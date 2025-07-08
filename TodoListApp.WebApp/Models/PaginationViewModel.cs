namespace TodoListApp.WebApp.Models;

public abstract class PaginationViewModel
{
    protected PaginationViewModel(int countOfItems, int currentPage = 1, int pageSize = 5)
    {
        this.PaginationInfo = new PaginationInfo()
        {
            CountOfItems = countOfItems,
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        if (currentPage < 1)
        {
            this.PaginationInfo.CurrentPage = 1;
        }
        else if (currentPage > this.PaginationInfo.TotalPages && this.PaginationInfo.TotalPages > 0)
        {
            this.PaginationInfo.CurrentPage = this.PaginationInfo.TotalPages;
        }
    }

    public PaginationInfo PaginationInfo { get; set; }
}
