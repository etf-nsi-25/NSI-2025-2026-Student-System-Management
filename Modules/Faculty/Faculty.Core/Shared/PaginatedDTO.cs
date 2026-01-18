namespace Faculty.Core.Shared
{
    public class PaginatedDTO<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }

}
