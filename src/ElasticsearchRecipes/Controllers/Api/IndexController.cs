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
        [HttpGet("file")]
        public async Task<IActionResult> IndexDataFromFile([FromQuery]string fileName, string index, bool deleteIndexIfExists)
        {
            var response = await this.indexer.IndexRecipesFromFile(fileName, deleteIndexIfExists, index);
            return Ok(response);
        }
    }
}
