using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundPlayType
{
    NONE = -1,
    BGM,
    EFFECT,
    UI
}

public class SoundClip
{
    public SoundPlayType playType = SoundPlayType.NONE;

    public int realID = 0;
    //Audio File
    private AudioClip clip = null;
    public string clipName = string.Empty;
    public string clipPath = string.Empty;

    public float maxVolume = 1.0f;

    //구간반복을 위한 변수
    public bool isLoop = false;
    public float[] checkTime = new float[0];
    public float[] setTime = new float[0];
    public int currentLoop = 0;

    public float pitch = 1.0f;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode audioRolloffMode = AudioRolloffMode.Logarithmic;

    //Audio Listener에 거리에 따른 소리관련 변수
    public float minDistance = 10000.0f;
    public float maxDistance = 50000.0f;
    public float spatialBlend = 1.0f;

    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;
    public Interpolate.Function interPolateFunction;
    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public SoundClip()
    {

    }

    public SoundClip(string _clipPath, string _clipName)
    {
        clipPath = _clipPath;
        clipName = _clipName;

        string pathLower = clipPath.ToLower();

        if (pathLower.Contains("bgm") == true)
        {
            playType = SoundPlayType.BGM;
        }
        else if (pathLower.Contains("effect") == true)
        {
            playType = SoundPlayType.EFFECT;
        }
        else if (pathLower.Contains("ui") == true)
        {
            playType = SoundPlayType.UI;
        }
        else
        {
            Debug.LogWarning("Can not Find Type : " + clipPath);
            playType = SoundPlayType.NONE;
        }
    }

    public void PreEnter()
    {
        if (clip == null)
        {
            string fullPath = clipPath + clipName;
            clip = ResourceManager.Load(fullPath) as AudioClip;
            if (clip == null)
            {
                Debug.LogWarning("PreEnter AudioClip Load Failed : " + fullPath);
            }
        }
    }

    public AudioClip GetClip()
    {
        if (clip == null)
        {
            PreEnter();
        }
        return clip;
    }

    public void ReleaseClip()
    {
        if (clip != null)
        {
            clip = null;
        }
    }

    //반복기능
    public void AddLoop()
    {
        checkTime = ArrayHelper.Add(0.0f, checkTime);
        setTime = ArrayHelper.Add(0.0f, setTime);
    }

    public void RemoveLoop(int _index)
    {
        checkTime = ArrayHelper.Remove(_index, checkTime);
        setTime = ArrayHelper.Remove(_index, setTime);
    }

    //반복구간을 가지고 있는지
    public bool HasLoop()
    {
        return checkTime.Length > 0;
    }

    //다음 반복구간으로 이동
    public void NextLoop()
    {
        currentLoop++;
        if (currentLoop >= checkTime.Length)
        {
            currentLoop = 0;
        }
    }

    public void CheckLoop(AudioSource _source)
    {
        //반복구간을 가지고 있고, 체크 타임보다 더 재생이 되었다면 세팅타임으로 시간을 돌림
        if (HasLoop() == true && _source.time >= checkTime[currentLoop])
        {
            _source.time = setTime[currentLoop];
            NextLoop();
        }
    }

    //fade 기능
    public void FadeIn(float _time, Interpolate.EaseType _easyType)
    {
        isFadeOut = false;
        isFadeIn = true;
        fadeTime1 = 0.0f;
        fadeTime2 = _time;
        interPolateFunction = Interpolate.Ease(_easyType);
    }

    public void FadeOut(float _time, Interpolate.EaseType _easyType)
    {
        isFadeOut = true;
        isFadeIn = false;
        fadeTime1 = 0.0f;
        fadeTime2 = _time;
        interPolateFunction = Interpolate.Ease(_easyType);
    }

    public void DoFade(float _time, AudioSource _audio)
    {
        if (isFadeIn == true)
        {
            fadeTime1 += _time;
            _audio.volume = Interpolate.Ease(interPolateFunction, 0, maxVolume, fadeTime1, fadeTime2);
            if (fadeTime1 >= fadeTime2)
            {
                isFadeIn = false;
            }
            else if (isFadeOut == true)
            {
                fadeTime1 += _time;
                _audio.volume = Interpolate.Ease(interPolateFunction, maxVolume, 0 - maxVolume, fadeTime1, fadeTime2);
                if (fadeTime1 >= fadeTime2)
                {
                    isFadeOut = false;
                    _audio.Stop();
                }
            }
        }
    }
}
