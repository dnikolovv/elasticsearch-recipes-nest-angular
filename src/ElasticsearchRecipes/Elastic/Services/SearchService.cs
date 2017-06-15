namespace ElasticsearchRecipes.Elastic.Services
{
    using Models;
    using Nest;
    using System.Text;
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

            BoolQueryDescriptor<Recipe> queryDescriptor = this.BuildQueryDescriptor(query);

            var response = await this.client.SearchAsync<Recipe>(r => r
                .Query(q => q
                    .Bool(b => queryDescriptor))
                                    .From(page * pageSize)
                                    .Size(pageSize));

            return new SearchResult<Recipe>
            {
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = page,
                Results = response.Documents
            };
        }

        private BoolQueryDescriptor<Recipe> BuildQueryDescriptor(string query)
        {
            string[] queryAsArray = query.Split();

            StringBuilder mustHaveParameters = new StringBuilder();
            StringBuilder mustNotHaveParameters = new StringBuilder();

            foreach (var word in queryAsArray)
            {
                // The words that are marked with a '-' sign in front musn't be present in the search results
                if (word.StartsWith("-"))
                {
                    // Get the word without the '-' in front
                    mustNotHaveParameters.Append($" {word.Substring(1, word.Length - 1)}");
                }
                else
                {
                    mustHaveParameters.Append($" {word}");
                }
            }

            BoolQueryDescriptor<Recipe> queryDescriptor = new BoolQueryDescriptor<Recipe>();

            queryDescriptor.Must(qc => qc.QueryString(qs => qs.Query(mustHaveParameters.ToString())));
            queryDescriptor.MustNot(qc => qc.QueryString(qs => qs.Query(mustNotHaveParameters.ToString())));

            return queryDescriptor;
        }
    }
}
