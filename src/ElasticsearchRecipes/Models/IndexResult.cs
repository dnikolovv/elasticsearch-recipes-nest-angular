namespace ElasticsearchRecipes.Models
{
    using System;

    public class IndexResult
    {
        public bool IsValid { get; set; }

        public string ErrorReason { get; set; }

        public Exception Exception { get; set; }
    }
}
