namespace ElasticsearchRecipes.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Just used as a shortcut to achieve a global route prefix.
    /// </summary>
    [Route("/api/[controller]")]
    public class ApiController : Controller
    {
    }
}
