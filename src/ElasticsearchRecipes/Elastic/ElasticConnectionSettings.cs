namespace ElasticsearchRecipes.Elastic
{
    public class ElasticConnectionSettings
    {
        public string ClusterUrl { get; set; }

        public string DefaultIndex
        {
            get { return defaultIndex; }
            set { defaultIndex = value.ToLower(); }
        }

        private string defaultIndex;
    }
}
