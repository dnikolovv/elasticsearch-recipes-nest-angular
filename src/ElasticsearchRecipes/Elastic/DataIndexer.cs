namespace ElasticsearchRecipes.Elastic
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using Models;
    using Nest;
    using Newtonsoft.Json;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataIndexer
    {
        public DataIndexer(ElasticClientProvider clientProvider, IHostingEnvironment env, IOptions<ElasticConnectionSettings> settings)
        {
            this.client = clientProvider.Client;
            this.contentRootPath = Path.Combine(env.ContentRootPath, "data");
            this.index = settings.Value.Index;
        }

        private readonly ElasticClient client;
        private readonly string contentRootPath;
        private readonly string index;

        public async Task<bool> IndexRecipesFromFile(string fileName, bool deleteIndexIfExists)
        {
            using (FileStream fs = new FileStream(Path.Combine(contentRootPath, fileName), FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string rawJsonCollection = await reader.ReadToEndAsync();

                    Recipe[] mappedCollection = JsonConvert.DeserializeObject<Recipe[]>(rawJsonCollection, new JsonSerializerSettings
                        {
                            Error = HandleDeserializationError
                        })
                        .Where(r => r.Name.Length > 0)
                        .ToArray();

                    // If the user specified to drop the index prior to indexing the documents
                    if (this.client.IndexExists(this.index).Exists && deleteIndexIfExists)
                    {
                        await this.client.DeleteIndexAsync(this.index);
                    }

                    if (!this.client.IndexExists(this.index).Exists)
                    {
                        var indexDescriptor = new CreateIndexDescriptor(this.index)
                                        .Mappings(mappings => mappings
                                            .Map<Recipe>(m => m.AutoMap()));

                        await this.client.CreateIndexAsync(this.index, i => indexDescriptor);
                    }

                    // Then index the documents
                    int batchSize = 10000; // magic
                    int totalBatches = mappedCollection.Length / batchSize;

                    for (int i = 0; i < totalBatches; i++)
                    {
                        var response = await this.client.IndexManyAsync(mappedCollection.Skip(i * batchSize).Take(batchSize));
                        System.Console.WriteLine($"Successfully indexed batch {i + 1}");
                        if (!response.IsValid)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        // https://stackoverflow.com/questions/26107656/ignore-parsing-errors-during-json-net-data-parsing
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
