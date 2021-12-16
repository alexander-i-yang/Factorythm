using System;
using UnityEngine;

public class BeatClip : MonoBehaviour
{
    public int BPM;
    public float SecPerBeat { get; private set; }
    public float BeatOffset;
    public AudioClip MusicClip;

    [NonSerialized] public double SongPosition = 0;
    [NonSerialized] public double SongPositionInBeats = 0;
    [NonSerialized] public int LastSongPosWholeBeats = 0;
    [NonSerialized] public double DSPSongTime = 0;

    //Controls time window for a valid input. Multiplied by seconds/beat
    [SerializeField] public double ValidTime { get; private set; } = 0.25;

    public void Init() {
        SecPerBeat = 60.0f/BPM;
        DSPSongTime = AudioSettings.dspTime;
    }
    
    public bool UpdateSongPos() {
        SongPosition = AudioSettings.dspTime-DSPSongTime-BeatOffset*SecPerBeat;
        // print(BeatOffset*SecPerBeat);
        SongPositionInBeats = SongPosition / SecPerBeat;
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
        return (SongPositionInBeats - LastSongPosWholeBeats)*SecPerBeat;
        
    }

    public double TimeTilNextBeat() {
        return SecPerBeat - TimeSinceBeat();
    }

    public bool IsOnBeat() {
        return TimeSinceBeat() < SecPerBeat*ValidTime || TimeTilNextBeat() < SecPerBeat*ValidTime;
    }
}
