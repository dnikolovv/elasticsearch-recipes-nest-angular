namespace ElasticsearchRecipes.Elastic
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using Models;
    using Nest;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
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

        public async Task<IndexResult> IndexRecipesFromFile(string fileName, bool deleteIndexIfExists, string index = null)
        {
            SanitizeIndexName(ref index);
            Recipe[] mappedCollection = await ParseJsonFile(fileName);
            await DeleteIndexIfExists(index, deleteIndexIfExists);
            await CreateIndexIfItDoesntExist(index);
            await ConfigurePagination(index);
            return await IndexDocuments(mappedCollection, index);
        }

        private void SanitizeIndexName(ref string index)
        {
            // The index must be lowercase, this is a requirement from Elastic
            if (index == null)
            {
                index = this.defaultIndex;
            }
            else
            {
                index = index.ToLower();
            }
        }

        private async Task<IndexResult> IndexDocuments(Recipe[] mappedCollection, string index)
        {
            int batchSize = 10000; // magic
            int totalBatches = (int)Math.Ceiling((double)mappedCollection.Length / batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var response = await this.client.IndexManyAsync(mappedCollection.Skip(i * batchSize).Take(batchSize), index);

                if (!response.IsValid)
                {
                    return new IndexResult
                    {
                        IsValid = false,
                        ErrorReason = response.ServerError?.Error?.Reason,
                        Exception = response.OriginalException
                    };
                }
                else
                {
                    Debug.WriteLine($"Successfully indexed batch {i + 1}");
                }
            }

            return new IndexResult
            {
                IsValid = true
            };
        }

        private async Task ConfigurePagination(string index)
        {
            // Max out the result window so you can have pagination for >100 pages
            await this.client.UpdateIndexSettingsAsync(index, ixs => ixs
                 .IndexSettings(s => s
                     .Setting("max_result_window", int.MaxValue)));
        }

        private async Task CreateIndexIfItDoesntExist(string index)
        {
            if (!this.client.IndexExists(index).Exists)
            {
                var indexDescriptor = new CreateIndexDescriptor(index)
                                .Mappings(mappings => mappings
                                    .Map<Recipe>(m => m.AutoMap()));

                await this.client.CreateIndexAsync(index, i => indexDescriptor);
            }
        }

        private async Task DeleteIndexIfExists(string index, bool shouldDeleteIndex)
        {
            if (this.client.IndexExists(index).Exists && shouldDeleteIndex)
            {
                await this.client.DeleteIndexAsync(index);
            }
        }

        private async Task<Recipe[]> ParseJsonFile(string fileName)
        {
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

                    return mappedCollection;
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
