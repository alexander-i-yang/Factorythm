using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe")]
public class RecipeScriptableObj : ScriptableObject {
    // public ResourceNum[] InCriteria;
    // public ResourceNum[] OutCriteria;
    public Criteria InCriteria;
    public Criteria OutCriteria;
    
    public int ticks;
    public bool CreatesNewOutput;
}