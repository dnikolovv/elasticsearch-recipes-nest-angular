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
            this.defaultIndex = settings.Value.DefaultIndex;
        }

        private readonly ElasticClient client;
        private readonly string contentRootPath;
        private readonly string defaultIndex;

        public async Task<bool> IndexRecipesFromFile(string fileName, bool deleteIndexIfExists, string index = null)
        {
            if (index == null)
            {
                index = this.defaultIndex;
            }

            using (FileStream fs = new FileStream(Path.Combine(contentRootPath, fileName), FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    // Won't be efficient with large files
                    string rawJsonCollection = await reader.ReadToEndAsync();

                    Recipe[] mappedCollection = JsonConvert.DeserializeObject<Recipe[]>(rawJsonCollection, new JsonSerializerSettings
                        {
                            Error = HandleDeserializationError
                        });

                    // If the user specified to drop the index prior to indexing the documents
                    if (this.client.IndexExists(index).Exists && deleteIndexIfExists)
                    {
                        await this.client.DeleteIndexAsync(index);
                    }

                    if (!this.client.IndexExists(index).Exists)
                    {
                        var indexDescriptor = new CreateIndexDescriptor(index)
                                        .Mappings(mappings => mappings
                                            .Map<Recipe>(m => m.AutoMap()));

                        await this.client.CreateIndexAsync(index, i => indexDescriptor);
                    }

                    // Max out the result window so you can have pagination for >100 pages
                    this.client.UpdateIndexSettings(index, ixs => ixs
                         .IndexSettings(s => s
                             .Setting("max_result_window", int.MaxValue)));

                    // Then index the documents
                    int batchSize = 10000; // magic
                    int totalBatches = (int)Math.Ceiling((double)mappedCollection.Length / batchSize);;

                    for (int i = 0; i < totalBatches; i++)
                    {
                        var response = await this.client.IndexManyAsync(mappedCollection.Skip(i * batchSize).Take(batchSize), index);
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
