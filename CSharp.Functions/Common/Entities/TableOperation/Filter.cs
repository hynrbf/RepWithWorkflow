namespace Common.Entities
{
    public class Filter
    {
        public string? Keywords { get; set; }

        //Sorter
        public string SortPropertyName { get; set; } = "";
        public string SortDirection { get; set; } = "asc";

        //Pager
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}