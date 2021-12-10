using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatClip : MonoBehaviour
{
    public int BPM;
    public float SecPerBeat { get; private set; }
    public float BeatOffset;
    public AudioClip MusicClip;

    [NonSerialized] public float SongPosition = 0;
    [NonSerialized] public float SongPositionInBeats = 0;
    [NonSerialized] public int LastSongPosWholeBeats = 0;
    [NonSerialized] public float DSPSongTime = 0;

    public void Init() {
        SecPerBeat = 60.0f/BPM;
        DSPSongTime = (float)AudioSettings.dspTime;
    }
    
    public bool UpdateSongPos() {
        SongPosition = (float) (AudioSettings.dspTime-DSPSongTime-BeatOffset*SecPerBeat);
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

    public float TimeSinceBeat() {
        return (SongPositionInBeats - LastSongPosWholeBeats)*SecPerBeat;
        
    }

    public float TimeTilNextBeat() {
        return SecPerBeat - TimeSinceBeat();
    }

    public bool IsOnBeat() {
        return TimeSinceBeat() < SecPerBeat/4 || TimeTilNextBeat() < SecPerBeat/4;
    }
}
