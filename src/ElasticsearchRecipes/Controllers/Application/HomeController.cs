namespace ElasticsearchRecipes.Controllers.Application
{
    using Microsoft.AspNetCore.Mvc;
    
    public class HomeController : Controller
    {
        public IActionResult Recipes()
        {
            return File("~/index.html", "text/html");
        }
    }
}
