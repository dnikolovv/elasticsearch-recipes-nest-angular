namespace ElasticsearchRecipes.Models
{
    using System;

    public class Recipe
    {
        public string Name { get; set; }

        public string Ingredients { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public string CookTime { get; set; }

        public string RecipeYield { get; set; }

        public DateTime DatePublished { get; set; }

        public string PrepTime { get; set; }

        public string Description { get; set; }
    }
}
