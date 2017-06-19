namespace ElasticsearchRecipes.Controllers.Api
{
    using Elastic;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class RecipeController : ApiController
    {
        public RecipeController(SearchService searchService)
        {
            this.searchService = searchService;
        }

        private readonly SearchService searchService;

        [HttpGet("search/{query?}")]
        public async Task<JsonResult> Search([FromQuery]string query, int page = 1, int pageSize = 10)
        {
            var result = await this.searchService.Search(query, page, pageSize);
            return Json(result);
        }

        [HttpGet("{id?}")]
        public async Task<JsonResult> GetById([FromQuery]string id)
        {
            var result = await this.searchService.GetById(id);
            return Json(result);
        }

        [HttpGet("morelikethis/{id?}")]
        public async Task<JsonResult> MoreLikeThis([FromQuery]string id, int page = 1, int pageSize = 10)
        {
            var result = await this.searchService.MoreLikeThis(id, page, pageSize);
            return Json(result);
        }

        [HttpGet("autocomplete/{query?}")]
        public async Task<JsonResult> Autocomplete([FromQuery]string query)
        {
            var result = await this.searchService.Autocomplete(query);
            return Json(result);
        }
    }
}
