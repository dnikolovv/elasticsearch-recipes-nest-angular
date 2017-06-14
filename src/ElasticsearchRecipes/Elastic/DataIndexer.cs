namespace ElasticsearchRecipes.Elastic
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Options;
    using Models;
    using Nest;
    using Newtonsoft.Json;
    using System.IO;
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

        public async Task IndexDataFromFile(string fileName)
        {
            using (FileStream fs = new FileStream(Path.Combine(contentRootPath, fileName), FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    // If you are using a very large file, you should avoid this method and read it in batches, because currently
                    // the whole file is being allocated into memory
                    string rawJsonCollection = await reader.ReadToEndAsync();
                    Recipe[] mappedCollection = JsonConvert.DeserializeObject<Recipe[]>(rawJsonCollection);

                    // First, clear the data to avoid indexing the same documents multiple times
                    await this.client.DeleteIndexAsync(this.index);
                    // Then index the documents
                    await this.client.IndexManyAsync(mappedCollection);
                }
            }
        }
    }
}
