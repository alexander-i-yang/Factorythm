using System;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeStyle {
    SPECIFIC = 0,
    MOVEMENT = 1,
}

/// <summary>
/// Represents a recipe for a machine. Contains its input criteria and output parameters.
/// </summary>
[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe")]
public class Recipe : ScriptableObject {
    // public ResourceNum[] InCriteria;
    // public ResourceNum[] OutCriteria;
    public ResourceCriteria InCriteria;
    public ResourceCriteria OutCriteria;
    public RecipeStyle Style;
    
    private static Func<Recipe, DictQueue<ResourceName, Resource>, Vector3, List<Resource>>[] RecipeStyles = {
        (recipe, dictQueue, position) => recipe.OutCriteria.ToList(position),
        (recipe, dictQueue, position) => dictQueue.ToList(),
    };

    public int ticks;

    public List<Resource> Create(DictQueue<ResourceName, Resource> d, Vector3 position) {
        return RecipeStyles[(int) Style](this, d, position);
    }
}