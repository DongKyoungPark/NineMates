using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicPlayType
{
    IDLE = -1,
    PLAY,
    PLAY_ONESHOT,
    STOP,
    FADE_IN,
    FADE_OUT,
    FADE_TO
}

public enum EventStartType
{
    INTERACT,
    AUTOSTART,
    TRIGGER_ENTER,
    TRIGGER_EXIT,
    NONE,
    KEY_PRESS,
    DROP
}

public enum AIConditionNeeded
{
    ALL,
    ONE
}

public enum ValueCheck
{
    EQUALS,
    LESS,
    GREATER
}

public enum SimpleOperator
{
    ADD,
    SUB,
    SET
}

public class BGMSoundPlayer : BaseInteraction
{
    public MusicPlayType playType = MusicPlayType.PLAY;
    new EventStartType eventStartType = EventStartType.NONE;

    public int musicClip_Index1 = 0;
    public int musicClip_Index2 = 1;

    public float fadeTime = 1.0f;

    public Interpolate.EaseType easeType = Interpolate.EaseType.Linear;

    public void PlayMusic()
    {
        switch (playType)
        {
            case MusicPlayType.PLAY:
                AudioManager.Instance.PlayBGM(musicClip_Index1);
                break;
            case MusicPlayType.PLAY_ONESHOT:
                AudioManager.Instance.PlayOneShot(musicClip_Index1);
                break;
            case MusicPlayType.FADE_IN:
                AudioManager.Instance.FadeIn(musicClip_Index1, fadeTime, easeType);
                break;
            case MusicPlayType.FADE_OUT:
                AudioManager.Instance.FadeOut(fadeTime, easeType);
                break;
            case MusicPlayType.FADE_TO:
                AudioManager.Instance.FadeTo(musicClip_Index1, fadeTime, easeType);
                break;
            case MusicPlayType.STOP:
                AudioManager.Instance.BGMStop();
                break;
        }
    }

    private void Start()
    {
        if (EventStartType.AUTOSTART.Equals(eventStartType))
        {
            PlayMusic();
        }
    }

    private void Update()
    {

    }
}
