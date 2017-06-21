namespace ElasticsearchRecipes.Elastic
{
    using Models;
    using Nest;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SearchService
    {
        public SearchService(ElasticClientProvider clientProvider)
        {
            this.client = clientProvider.Client;
        }

        private readonly ElasticClient client;

        /// <summary>
        /// Searches elastic for recipes matching the given query. If a word in the query is preceeded by a '-' sign, the results won't contain it. Supports wildcard queries.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<SearchResult<Recipe>> Search(string query, int page, int pageSize)
        {
            /*
            Raw query:
            {
              "from": (page - 1) * pageSize,
              "size": pageSize,
              "query": {
                "bool": {
                  "must": [
                    {
                      "query_string": {
                        "query": "words without a '-' in front"
                      }
                    }
                  ],
                  "must_not": [
                    {
                      "query_string": {
                        "query": "words with a '-' in front"
                      }
                    }
                  ]
                }
              }
            }
            */

            BoolQueryDescriptor<Recipe> queryDescriptor = this.BuildQueryDescriptor(query);

            var response = await this.client.SearchAsync<Recipe>(r => r
                    .Query(q => q
                        .Bool(b => queryDescriptor))
                                        .From((page - 1) * pageSize)
                                        .Size(pageSize));

            // TODO: Should check if response is valid
            return new SearchResult<Recipe>
            {
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = page,
                Results = response.Documents
            };
        }

        public async Task<SearchResult<Recipe>> MoreLikeThis(string id, int page, int pageSize)
        {
            /*
            Raw query:
            {
              "from": (page - 1) * pageSize,
              "size": pageSize,
              "query": {
                "more_like_this": {
                  "fields": [
                    "ingredients"
                  ],
                  "like": [
                    {
                      "_index": "recipes",
                      "_type": "recipe",
                      "_id": "id"
                    }
                  ]
                }
              }
            }
            */

            var response = await this.client.SearchAsync<Recipe>(s => s
                .Query(q => q
                    .MoreLikeThis(qd => qd
                        .Like(l => l.Document(d => d.Id(id)))
                        .Fields(fd => fd.Fields(r => r.Ingredients))))
                        .From((page - 1) * pageSize)
                        .Size(pageSize));

            // TODO: Should check if response is valid
            return new SearchResult<Recipe>
            {
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = page,
                Results = response.Documents
            };
        }

        public async Task<Recipe> GetById(string id)
        {
            var response = await this.client.GetAsync<Recipe>(id);
            return response.Source;
        }

        public async Task<List<string>> Autocomplete(string query)
        {
            /*
            Raw query:
            {
              "suggest": {
                "recipe-name-completion": {
                  "prefix": "query",
                  "completion": {
                    "field": "name",
                    "fuzzy": {
                      "fuzziness": "AUTO"
                    }
                  }
                }
              }
            }
            */

            var completionQuery = await this.client.SearchAsync<Recipe>(sr => sr
                .Suggest(scd => scd
                    .Completion("recipe-name-completion", cs => cs
                        .Prefix(query)
                        .Fuzzy(fsd => fsd
                            .Fuzziness(Fuzziness.Auto))
                        .Field(r => r.Name))));

            var matchingOptions = completionQuery.Suggest["recipe-name-completion"].Select(s => s.Options);

            List<string> results = new List<string>();

            foreach (var option in matchingOptions)
            {
                results.AddRange(option.Select(opt => opt.Text));
            }

            return results;
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
