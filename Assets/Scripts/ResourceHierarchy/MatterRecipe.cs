public class MatterRecipe : Recipe {
    protected override bool _isInputValidR(Resource recipeResource, Resource compareAgainst) {
        return recipeResource.matterState == compareAgainst.matterState;
    }
}