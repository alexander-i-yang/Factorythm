//TODO: Delete this class and implement the selling of items through Recipes.

/// <summary>
/// <b>Legacy.</b> A machine that sells resources instead of chaning them/producing output.
/// </summary>
public class SellMachine : Machine {
    //Shouldn't destroy bc we need to read resoruces in in order to sell them
    protected override bool _shouldDestroyInputs() {
        return false;
    }

    protected override void CreateOutput() {
        foreach (Resource r in OutputBuffer) {
            Conductor.Instance.Sell(r);
        }
    }
    
    protected override void _produce() {
        MoveResourcesIn();
        CreateOutput();
        foreach (var resource in OutputBuffer) {
            Destroy(resource.gameObject);
        }
        OutputBuffer.Clear();
    }
}