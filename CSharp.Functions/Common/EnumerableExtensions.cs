namespace Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SortByProperty<T>(this IEnumerable<T> source, string propertyName,
            bool isDescending = false)
        {
            var properties = typeof(T).GetProperties();
            var propertyInfo = Array.Find(properties,
                p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (propertyInfo == null)
            {
                return source;
            }

            return isDescending
                ? source.OrderByDescending(x => propertyInfo.GetValue(x, null))
                : source.OrderBy(x => propertyInfo.GetValue(x, null));
        }

        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int currentPage, int pageSize)
        {
            if (currentPage <= 0 || pageSize <= 0)
            {
                return source;
            }

            var startIndex = (currentPage - 1) * pageSize;

            var pageData = source.Skip(startIndex).Take(pageSize);

            return pageData.ToList();
        }
    }
}
