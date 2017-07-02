namespace ElasticsearchRecipes.Elastic
{
    using Models;
    using Nest;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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
        /// Supports phrase matching when the phrase is surrounded by quotes.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<SearchResult<Recipe>> Search(string query, int page, int pageSize)
        {
            // TODO: Raw query is not updated after the switch to QueryStringQuery!!
            #region RawQuery
            /* 
            Passed query: "chopped onions" -"dried beans" eggs -pepper
            {
              "from": 0,
              "size": 10,
              "query": {
                "bool": {
                  "must": [
                    {
                      "query_string": {
                        "query": "eggs" // A term that isn't marked with anything
                      }
                    },
                    {
                      "multi_match": {
                        "type": "phrase",
                        "query": "\"chopped onions\"", // A phrase that is marked with quotes
                        "fields": [
                          "name",
                          "ingredients"
                        ]
                      }
                    }
                  ],
                  "must_not": [
                    {
                      "query_string": {
                        "query": "pepper" // A term that is marked with a '-' sign in front
                      }
                    },
                    {
                      "multi_match": {
                        "type": "phrase",
                        "query": "\"dried beans\"", // A phrase that is marked with a '-' sign in front
                        "fields": [
                          "name",
                          "ingredients"
                        ]
                      }
                    }
                  ]
                }
              }
            }
            */
            #endregion

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
                PageSize = pageSize,
                Results = response.Documents
            };
        }

        public async Task<SearchResult<Recipe>> MoreLikeThis(string id, int page, int pageSize)
        {
            #region RawQuery
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
            #endregion

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
                PageSize = pageSize,
                Results = response.Documents
            };
        }

        public async Task<Recipe> GetById(string id)
        {
            var response = await this.client.GetAsync<Recipe>(id);
            return response.Source;
        }

        public async Task<List<AutocompleteResult>> Autocomplete(string query)
        {
            #region RawQuery
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
            #endregion

            var completionQuery = await this.client.SearchAsync<Recipe>(sr => sr
                .Suggest(scd => scd
                    .Completion("recipe-name-completion", cs => cs
                        .Prefix(query)
                        .Fuzzy(fsd => fsd
                            .Fuzziness(Fuzziness.Auto))
                        .Field(r => r.Name))));

            var matchingOptions = completionQuery.Suggest["recipe-name-completion"].Select(s => s.Options);

            List<AutocompleteResult> results = new List<AutocompleteResult>();

            foreach (var option in matchingOptions)
            {
                results.AddRange(option.Select(opt => new AutocompleteResult() { Id = opt.Source.Id, Name = opt.Source.Name }));
            }

            return results;
        }

        private BoolQueryDescriptor<Recipe> BuildQueryDescriptor(string query)
        {
            // Get the terms and phrases
            string[] terms = ExtractTerms(query);
            string[] phrases = ExtractPhrases(query);

            List<QueryContainer> mustClauses = new List<QueryContainer>();
            List<QueryContainer> mustNotClauses = new List<QueryContainer>();

            // Traverse the query params and collect all must/must_not clauses
            foreach (var param in terms.Concat(phrases))
            {
                QueryStringQuery queryToAdd = new QueryStringQuery();

                if (param.StartsWith("-"))
                {
                    queryToAdd.Query = param.Substring(1, param.Length - 1);
                    mustNotClauses.Add(queryToAdd);
                }
                else
                {
                    queryToAdd.Query = param;
                    mustClauses.Add(queryToAdd);
                }
            }

            // Create the actual query descriptor
            BoolQueryDescriptor<Recipe> queryDescriptor = new BoolQueryDescriptor<Recipe>();
            // Assign to it the clauses that we collected while processing the query
            queryDescriptor.Must(mustClauses.ToArray());
            queryDescriptor.MustNot(mustNotClauses.ToArray());
            // Return it
            return queryDescriptor;
        }

        private string[] ExtractPhrases(string query)
        {
            // If it's surrounded by " then " or surrounded by -" then "
            Regex reg = new Regex("\\-\".*?\\\"|\".*?\\\"");

            string[] phrases = reg.Matches(query)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            return phrases;
        }

        private string[] ExtractTerms(string query)
        {
            StringBuilder terms = new StringBuilder();

            foreach (var word in query.Split())
            {
                // If it doesn't start/end with a quote or a minus quote (-")
                // Both would indicate that it is marked as a phrase and not a term ("example phrase", -"example must_not phrase")
                if (!word.StartsWith("\"") && !word.StartsWith("-\"") && !word.EndsWith("\""))
                {
                    terms.Append($" {word}");
                }
            }

            return terms.ToString().Split();
        }
    }
}
