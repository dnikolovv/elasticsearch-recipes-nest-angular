namespace ElasticsearchRecipes.Controllers.Api
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

        /// <summary>
        /// The file must be present in the project Data directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="deleteIndexIfExists"></param>
        /// <returns></returns>
        [HttpGet("file/{fileName}")]
        public async Task<IActionResult> IndexDataFromFile(string fileName, bool deleteIndexIfExists)
        {
            // TODO: Handle exceptions
            var response = await this.indexer.IndexRecipesFromFile(fileName, deleteIndexIfExists);
            return Ok(response);
        }
    }
}
