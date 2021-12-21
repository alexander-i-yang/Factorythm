using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe")]
public class RecipeScriptableObj : ScriptableObject {
    public ResourceNum[] InResources;
    public ResourceNum[] OutResources;

    public int ticks;
    public bool CreatesNewOutput;
}