public class ExactRecipe : Recipe {
    protected override bool _isInputValidR(Resource recipeResource, Resource compareAgainst) {
        return recipeResource.ResourceName == compareAgainst.ResourceName;
    }
}