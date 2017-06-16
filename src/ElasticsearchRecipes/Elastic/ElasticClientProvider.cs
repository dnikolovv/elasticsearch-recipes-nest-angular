namespace ElasticsearchRecipes.Elastic
{
    using Microsoft.Extensions.Options;
    using Nest;

    public class ElasticClientProvider
    {
        public ElasticClientProvider(IOptions<ElasticConnectionSettings> settings)
        {
            ConnectionSettings connectionSettings =
                new ConnectionSettings(new System.Uri(settings.Value.ClusterUrl));
            
            connectionSettings.EnableDebugMode();

            if (settings.Value.Index != null)
            {
                connectionSettings.DefaultIndex(settings.Value.Index);
            }

            this.Client = new ElasticClient(connectionSettings);
        }

        public ElasticClient Client { get; }
    }
}
