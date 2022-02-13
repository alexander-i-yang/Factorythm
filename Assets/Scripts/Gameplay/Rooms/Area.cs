/// <summary>
/// Interface that defines this object as a room/area.
/// </summary>
public interface Area {
    
    //Returns true if you can place the machine in this area.
    bool CanPlaceHere(Machine m);
}
