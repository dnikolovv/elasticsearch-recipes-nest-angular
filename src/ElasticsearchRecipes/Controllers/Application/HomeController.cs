namespace ElasticsearchRecipes.Controllers.Application
{
    using Microsoft.AspNetCore.Mvc;

    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Recipes()
        {
            return View();
        }
    }
}
