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
    
    //TODO: figure out a better way to do this. Rn it completely overrides the produce method. Very spaghetti.
    protected override void _produce() {
        MoveResourcesIn();
        CreateOutput();
        foreach (var resource in OutputBuffer) {
            Destroy(resource.gameObject);
        }
        OutputBuffer.Clear();
    }
}