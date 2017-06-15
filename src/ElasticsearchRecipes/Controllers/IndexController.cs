namespace ElasticsearchRecipes.Controllers
{
    using Elastic;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class IndexController : ApiController
    {
        public IndexController(DataIndexer indexer)
        {
            this.indexer = indexer;
        }

        private readonly DataIndexer indexer;

        [HttpGet("file/{fileName}")]
        public async Task<IActionResult> IndexDataFromFile(string fileName, bool deleteIndexIfExists)
        {
            // TODO: Handle exceptions
            var response = await this.indexer.IndexRecipesFromFile(fileName, deleteIndexIfExists);
            return Ok(response);
        }
    }
}
