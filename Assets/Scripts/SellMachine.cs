public class SellMachine : Machine{
    protected override void CreateOutput() {
        foreach (Resource r in OutputBuffer)
        {
            print(r.price);
        }
    }
}