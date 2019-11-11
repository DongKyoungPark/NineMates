using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : Singleton<AudioManager>, NM.IManager
{
    public const string MasterGroupName = "Master";
    public const string EffectGroupName = "Effect";
    public const string BGMGroupName = "BGM";
    public const string UIGroupName = "UI";
    public const string MixerName = "AudioMixer";
    public const string ContainerName = "SoundContainer";
    public const string FadeA = "FadeA";
    public const string FadeB = "FadeB";
    public const string UI = "UI";
    public const string EffectVolumeParam = "Volume_Effect";
    public const string BGMVolumeParam = "Volume_BGM";
    public const string UIVolumeParam = "Volume_UI";

    public enum MusicPlayingType { None = 0, SourceA = 1, SourceB = 2, AtoB = 3, BtoA = 4 }

    public AudioMixer mixer = null;
    public Transform audioRoot = null;
    public AudioSource fadeA_audio = null;
    public AudioSource fadeB_audio = null;
    public AudioSource[] effect_audios = null;
    public AudioSource UI_audio = null;

    public float[] effect_PlayStartTime = null;
    private int effectChannelCount = 5;
    private MusicPlayingType currentPlayingType = MusicPlayingType.None;
    private bool isTicking = false;
    private SoundClip currentSound = null;
    private SoundClip lastSound = null;
    private bool nowMute = false;
    private float lastEffectVolume = 0.0f;
    private float lastUIVolume = 0.0f;
    private float lastBGMVolume = 0.0f;
    private float minVolume = -80.0f;
    private float maxVolume = 0.0f;

    private AudioListener audioListener = null;

    // Start is called before the first frame update
    void Start()
    {
        if (mixer == null)
        {
            mixer = ResourceManager.Load(MixerName) as AudioMixer;
        }
        if (audioRoot == null)
        {
            audioRoot = new GameObject(ContainerName).transform;
            audioRoot.SetParent(transform);
            audioRoot.localPosition = Vector3.zero;
        }
        if (fadeA_audio == null)
        {
            GameObject fadeA_GO = new GameObject(FadeA, typeof(AudioSource));
            fadeA_GO.transform.SetParent(audioRoot);
            fadeA_audio = fadeA_GO.GetComponent<AudioSource>();
            fadeA_audio.playOnAwake = false;
        }
        if (fadeB_audio == null)
        {
            GameObject fadeB_GO = new GameObject(FadeB, typeof(AudioSource));
            fadeB_GO.transform.SetParent(audioRoot);
            fadeB_audio = fadeB_GO.GetComponent<AudioSource>();
            fadeB_audio.playOnAwake = false;
        }
        if (UI_audio == null)
        {
            GameObject UI_GO = new GameObject(UI, typeof(AudioSource));
            UI_GO.transform.SetParent(audioRoot);
            UI_audio = UI_GO.GetComponent<AudioSource>();
            UI_audio.playOnAwake = false;
        }
        if (effect_audios == null)
        {
            effect_PlayStartTime = new float[effectChannelCount];
            effect_audios = new AudioSource[effectChannelCount];
            for (int i = 0; i < effectChannelCount; i++)
            {
                effect_PlayStartTime[i] = 0.0f;
                GameObject Effect_GO = new GameObject("Effect_" + i.ToString(), typeof(AudioSource));
                Effect_GO.transform.SetParent(audioRoot);
                effect_audios[i] = Effect_GO.GetComponent<AudioSource>();
                effect_audios[i].playOnAwake = false;
            }
        }
        if (mixer != null)
        {
            fadeA_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            fadeB_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];

            UI_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(UIGroupName)[0];
            for (int i = 0; i < effect_audios.Length; i++)
            {
                effect_audios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(EffectGroupName)[0];
            }
        }
        if (audioListener == null)
        {
            audioListener = gameObject.AddComponent<AudioListener>();
        }

        //볼륨초기화.
        Init();
    }
    public void SetBGMVolume(float _currentRatio)//ui Slider 0 ~ 1.
    {
        _currentRatio = Mathf.Clamp01(_currentRatio);//0~1로 세팅.

        float volume = Mathf.Lerp(minVolume, maxVolume, _currentRatio);
        mixer.SetFloat(BGMVolumeParam, volume);
        PlayerPrefs.SetFloat(BGMVolumeParam, volume);

    }
    public float GetBGMVolume()
    {
        if (PlayerPrefs.HasKey(BGMVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(BGMVolumeParam);
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetEffectVolume(float _currentRatio)
    {
        _currentRatio = Mathf.Clamp01(_currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, _currentRatio);
        mixer.SetFloat(EffectVolumeParam, volume);
        PlayerPrefs.SetFloat(EffectVolumeParam, volume);
    }
    public float GetEffectVolume()
    {
        if (PlayerPrefs.HasKey(EffectVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(EffectVolumeParam);
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetUIVolume(float _currentRatio)
    {
        _currentRatio = Mathf.Clamp01(_currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, _currentRatio);
        mixer.SetFloat(UIVolumeParam, volume);
        PlayerPrefs.SetFloat(UIVolumeParam, volume);
    }
    public float GetUIVolume()
    {
        if (PlayerPrefs.HasKey(UIVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(UIVolumeParam);
        }
        else
        {
            return maxVolume;
        }

    }

    void PlayAudioSource(AudioSource _source, SoundClip _clip, float _volume)
    {
        if (_source == null || _clip == null)
        {
            Debug.LogWarning("Plz check this PlayAudioSource");
            return;
        }
        _source.Stop();
        _source.clip = _clip.GetClip();
        _source.volume = _volume;
        _source.loop = _clip.isLoop;
        _source.pitch = _clip.pitch;
        _source.dopplerLevel = _clip.dopplerLevel;
        _source.rolloffMode = _clip.audioRolloffMode;
        _source.minDistance = _clip.minDistance;
        _source.maxDistance = _clip.maxDistance;
        _source.spatialBlend = _clip.spatialBlend;
        _source.Play();
    }
    /// <summary>
    /// 현재 사운드를 재생중입니까?
    /// </summary>
    public bool IsPlaying()
    {
        return (int)currentPlayingType > 0;
    }
    public bool IsDifferentSound(SoundClip _clip)
    {
        if (_clip == null)
        {
            return false;
        }
        if (currentSound != null && currentSound.realID == _clip.realID && IsPlaying() && currentSound.isFadeOut == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator CheckProcess()
    {
        while (isTicking == true && IsPlaying() == true)
        {
            yield return new WaitForSeconds(0.05f);
            if (currentSound.HasLoop())
            {
                switch (currentPlayingType)
                {
                    case MusicPlayingType.SourceA:
                        currentSound.CheckLoop(fadeA_audio);
                        break;
                    case MusicPlayingType.SourceB:
                        currentSound.CheckLoop(fadeB_audio);
                        break;
                    case MusicPlayingType.AtoB:
                        lastSound.CheckLoop(fadeA_audio);
                        currentSound.CheckLoop(fadeB_audio);
                        break;
                    case MusicPlayingType.BtoA:
                        lastSound.CheckLoop(fadeB_audio);
                        currentSound.CheckLoop(fadeA_audio);
                        break;
                }
            }
        }
    }
    public void DoCheck()
    {
        StartCoroutine(CheckProcess());
    }

    public void FadeIn(SoundClip _clip, float _time, Interpolate.EaseType _ease)
    {
        if (IsDifferentSound(_clip) == true)
        {
            fadeA_audio.Stop();
            fadeB_audio.Stop();
            lastSound = currentSound;
            currentSound = _clip;

            PlayAudioSource(fadeA_audio, currentSound, 0.0f);

            currentSound.FadeIn(_time, _ease);
            currentPlayingType = MusicPlayingType.SourceA;
            if (currentSound.HasLoop() == true)
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeIn(int _index, float _time, Interpolate.EaseType _ease)
    {
        FadeIn(DataManager.Sound().GetCopy(_index), _time, _ease);
    }

    public void FadeOut(float _time, Interpolate.EaseType _ease)
    {
        if (currentSound != null)
        {
            currentSound.FadeOut(_time, _ease);
        }
    }

    private void Update()
    {
        if (currentSound == null)
        {
            return;
        }

        switch (currentPlayingType)
        {
            case MusicPlayingType.SourceA:
                currentSound.DoFade(Time.deltaTime, fadeA_audio);
                break;
            case MusicPlayingType.SourceB:
                currentSound.DoFade(Time.deltaTime, fadeB_audio);
                break;
            case MusicPlayingType.AtoB:
                lastSound.DoFade(Time.deltaTime, fadeA_audio);
                currentSound.DoFade(Time.deltaTime, fadeB_audio);
                break;
            case MusicPlayingType.BtoA:
                lastSound.DoFade(Time.deltaTime, fadeB_audio);
                currentSound.DoFade(Time.deltaTime, fadeA_audio);
                break;

        }

        if (fadeA_audio.isPlaying == true && fadeB_audio.isPlaying == false)
        {
            currentPlayingType = MusicPlayingType.SourceA;
        }
        else if (fadeB_audio.isPlaying == true && fadeA_audio.isPlaying == false)
        {
            currentPlayingType = MusicPlayingType.SourceB;
        }
        else if (fadeA_audio.isPlaying == false && fadeB_audio.isPlaying == false)
        {
            currentPlayingType = MusicPlayingType.None;
        }
    }

    public void FadeTo(SoundClip _clip, float _time, Interpolate.EaseType _ease)
    {
        if (currentPlayingType == MusicPlayingType.None)
        {
            FadeIn(_clip, _time, _ease);
        }
        else if (IsDifferentSound(_clip) == true)
        {
            if (currentPlayingType == MusicPlayingType.AtoB)
            {
                fadeA_audio.Stop();
                currentPlayingType = MusicPlayingType.SourceB;
            }
            else if (currentPlayingType == MusicPlayingType.BtoA)
            {
                fadeB_audio.Stop();
                currentPlayingType = MusicPlayingType.SourceA;
            }

            //fade to
            lastSound = currentSound;
            currentSound = _clip;
            lastSound.FadeOut(_time, _ease);
            currentSound.FadeIn(_time, _ease);
            if (currentPlayingType == MusicPlayingType.SourceA)
            {
                PlayAudioSource(fadeB_audio, currentSound, 0.0f);
                currentPlayingType = MusicPlayingType.AtoB;
            }
            else if (currentPlayingType == MusicPlayingType.SourceB)
            {
                PlayAudioSource(fadeA_audio, currentSound, 0.0f);
                currentPlayingType = MusicPlayingType.BtoA;
            }
            if (currentSound.HasLoop())
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeTo(int _index, float _time, Interpolate.EaseType _ease)
    {
        FadeTo(DataManager.Sound().GetCopy(_index), _time, _ease);
    }

    //client api
    public void PlayBGM(SoundClip _clip)
    {
        if (IsDifferentSound(_clip) == true)
        {
            fadeB_audio.Stop();
            lastSound = currentSound;
            currentSound = _clip;
            PlayAudioSource(fadeA_audio, _clip, 1.0f);
            if (currentSound.HasLoop() == true)
            {
                isTicking = true;
                DoCheck();
            }
        }
    }
    public void PlayBGM(int _index)
    {
        PlayBGM(DataManager.Sound().GetCopy(_index));
    }

    public void PlayUISound(SoundClip _clip)
    {
        PlayAudioSource(UI_audio, _clip, 1.0f);
    }

    public void PlayEffectSound(SoundClip _clip)
    {
        bool isPlaySuccess = false;

        for (int i = 0; i < effectChannelCount; i++)
        {
            //빈 사운드 채널이 있다면.
            if (effect_audios[i].isPlaying == false)
            {
                PlayAudioSource(effect_audios[i], _clip, 0.0f);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
            else if (effect_audios[i].clip == _clip.GetClip())
            {
                effect_audios[i].Stop();
                PlayAudioSource(effect_audios[i], _clip, 0.0f);
                effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
        }

        if (isPlaySuccess == false)
        {
            float maxTime = 0.0f;
            int selectIndex = 0;
            for (int i = 0; i < effectChannelCount; i++)
            {
                if (effect_PlayStartTime[i] > maxTime)
                {
                    maxTime = effect_PlayStartTime[i];
                    selectIndex = i;
                }
            }
            PlayAudioSource(effect_audios[selectIndex], _clip, 0.0f);
        }
    }

    public void PlayOneShot(SoundClip _clip)
    {
        if (_clip == null)
        {
            Debug.LogWarning("Play One shot param clip is null!!");
            return;
        }
        switch (_clip.playType)
        {
            case SoundPlayType.EFFECT:
                PlayEffectSound(_clip);
                break;
            case SoundPlayType.BGM:
                PlayBGM(_clip);
                break;
            case SoundPlayType.UI:
                PlayUISound(_clip);
                break;
        }
    }

    public void PlayOneShot(int _index)
    {
        if (_index == (int)SoundList.None)
        {
            return;
        }
        PlayOneShot(DataManager.Sound().GetCopy(_index));
    }
    //BGM STOP
    public void BGMStop(bool _allStop = false)
    {
        if (_allStop == true)
        {
            fadeA_audio.Stop();
            fadeB_audio.Stop();
        }
        FadeOut(0.5f, Interpolate.EaseType.Linear);
        currentPlayingType = MusicPlayingType.None;
        isTicking = false;
        StopAllCoroutines();
    }


    /// <summary>
    /// 볼륨을 초기화합니다.
    /// </summary>
    public void Init()
    {
        if (mixer != null)
        {
            mixer.SetFloat(BGMVolumeParam, GetBGMVolume());
            mixer.SetFloat(EffectVolumeParam, GetEffectVolume());
            mixer.SetFloat(UIVolumeParam, GetUIVolume());
        }
    }

    public void Release()
    {

    }
}
