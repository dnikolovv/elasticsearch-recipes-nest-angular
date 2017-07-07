namespace ElasticsearchRecipes.Controllers.Application
{
    using Microsoft.AspNetCore.Mvc;
    
    public class HomeController : Controller
    {
        // A catch-all route is registered in Startup.cs so that every url leads to this action
        public IActionResult Recipes()
        {  
            // The angular app entry point
            return File("~/index.html", "text/html");
        }
    }
}
