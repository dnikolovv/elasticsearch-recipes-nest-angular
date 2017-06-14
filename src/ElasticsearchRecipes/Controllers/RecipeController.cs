namespace ElasticsearchRecipes.Controllers
{
    using Elastic.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class RecipeController : ApiController
    {
        public RecipeController(SearchService searchService)
        {
            this.searchService = searchService;
        }

        private readonly SearchService searchService;

        [HttpGet("{query}")]
        public async Task<JsonResult> Search(string query, int page = 1, int pageSize = 10)
        {
            var result = await this.searchService.Search(query, page, pageSize);
            return Json(result);
        }
    }
}
