/// <summary>
/// Handles all logic for when the song is offbeat.
/// </summary>
public class OffBeatState : BeatState {
    public override void Enter(BeatStateInput i) {
        
    }

    public override void Exit(BeatStateInput i) {
    }

    public override void Update(BeatStateInput input) {
        if (Conductor.Instance.SongIsOnBeat()) {
            MyStateMachine.Transition<OnBeatState>();
        }
    }

    public override bool AttemptMove(BeatStateInput input) {
        Conductor.Instance.ResetCombo();
        return !Conductor.Instance.RhythmLock;
    }
}