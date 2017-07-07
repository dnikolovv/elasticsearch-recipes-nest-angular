namespace ElasticsearchRecipes.Models
{
    using Nest;
    using System;

    [ElasticsearchType(Name = "recipe")]
    public class Recipe
    {
        public string Id { get; set; }

        [Completion]
        public string Name { get; set; }
        [Text]
        public string Ingredients { get; set; }
        
        public string Url { get; set; }

        public string Image { get; set; }

        public string CookTime { get; set; }
        
        public string RecipeYield { get; set; }

        public DateTime? DatePublished { get; set; }

        public string PrepTime { get; set; }
        [Text]
        public string Description { get; set; }
    }
}
