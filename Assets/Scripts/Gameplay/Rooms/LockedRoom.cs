public class LockedRoom : Room {
    public override bool CanPlayerEnter(PlayerController pc) {
        return false;
    }

    public override bool CanPlaceHere(Machine m) {
        return false;
    }
}