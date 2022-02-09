using System;
using UnityEngine;

public class BeatClipHelper {
    [NonSerialized] public double SongPosition = 0;
    [NonSerialized] public double SongPositionInBeats = 0;
    [NonSerialized] public int LastSongPosWholeBeats = 0;
    // [NonSerialized] public double PlayStartTime = 0;
    public BeatClipSO BeatClip { get; private set; }
    
    //Controls time window for a valid input. Multiplied by seconds/beat
    [SerializeField] public double ValidTime { get; private set; } = 0.25;

    public void Reset(BeatClipSO clip) {
        BeatClip = clip;
        BeatClip.SecPerBeat = 60.0f/BeatClip.BPM;
        // PlayStartTime = Time.time;
        SongPosition = 0;
        SongPositionInBeats = 0;
        LastSongPosWholeBeats = 0;
    }
    
    public bool UpdateSongPos(float audioTime) {
        // SongPosition = AudioSettings.dspTime-PlayStartTime-BeatClip.BeatOffset*BeatClip.SecPerBeat;
        SongPosition = 
            audioTime
            // TimeTime.time - PlayStartTime
            -BeatClip.BeatOffset*BeatClip.SecPerBeat;
        // print(BeatOffset*SecPerBeat);
        SongPositionInBeats = SongPosition / BeatClip.SecPerBeat;
        bool isNewBeat = IsNewBeat();
        if (isNewBeat) {
            LastSongPosWholeBeats = (int) Math.Floor(SongPositionInBeats);
        }
        return isNewBeat;
    }

    public bool IsNewBeat() {
        return SongPositionInBeats - LastSongPosWholeBeats > 1;
    }

    public double TimeSinceBeat() {
        return (SongPositionInBeats - LastSongPosWholeBeats)*BeatClip.SecPerBeat;
        
    }

    public double TimeTilNextBeat() {
        return BeatClip.SecPerBeat - TimeSinceBeat();
    }

    public bool IsOnBeat() {
        return TimeSinceBeat() < BeatClip.SecPerBeat*ValidTime || TimeTilNextBeat() < BeatClip.SecPerBeat*ValidTime;
    }
}
