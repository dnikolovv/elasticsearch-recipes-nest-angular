namespace ElasticsearchRecipes.Models
{
    using System.Collections.Generic;

    public class SearchResult<T>
    {
        public long Total { get; set; }

        public int Page { get; set; }

        public IEnumerable<T> Results { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
