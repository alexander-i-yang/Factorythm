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
        foreach (var resource in OutputBuffer) {
            print(resource);
        }
        CreateOutput();
        foreach (var resource in OutputBuffer) {
            Destroy(resource.gameObject);
        }
        OutputBuffer.Clear();
    }
}