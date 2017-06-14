namespace ElasticsearchRecipes.Elastic.Services
{
    using Models;
    using Nest;
    using System.Threading.Tasks;

    public class SearchService
    {
        public SearchService(ElasticClientProvider clientProvider)
        {
            this.client = clientProvider.Client;
        }

        private readonly ElasticClient client;

        // TODO: Implement must_not query
        public async Task<SearchResult<Recipe>> Search(string query, int page, int pageSize)
        {
            var matches = await this.client.SearchAsync<Recipe>(r => r
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m.QueryString(qs => qs
                            .Query(query)
                                .AnalyzeWildcard(true)))))
                                    .From(page * pageSize)
                                    .Size(pageSize));

            return new SearchResult<Recipe>
            {
                Total = matches.Total,
                ElapsedMilliseconds = matches.Took,
                Page = page,
                Results = matches.Documents
            };
        }
    }
}
