using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip soundClip;
    private AudioSource soundSource;

    public float volume;
    public bool loop;

    public void SetSoundSource(AudioSource _source)
    {
        soundSource = _source;
        soundSource.clip = soundClip;
        soundSource.volume = volume;
        soundSource.loop = loop;
    }

    public void Play(bool canReplay = false)
    {
        if (soundSource.isPlaying && canReplay == false)
            return;
        else
            soundSource.Play();
    }

    public void Stop()
    {
        soundSource.Stop();
    }

    public void SetLoop()
    {
        soundSource.loop = true;
    }

    public void SetLoopCancle()
    {
        soundSource.loop = false;
    }

    public void SetVolume(float _volume)
    {
        soundSource.volume = _volume;
    }
}

public class SoundManager : Singleton<SoundManager>, NM.IManager
{
    static public SoundManager instance;

    [Header("BGM 사운드")]
    [Header("=================================================================")]
    public string introBGM;
    public string mainBGM;
    public string inGameBGM;

    [Header("Effect 사운드")]
    [Header("=================================================================")]
    public string walkSound;
    public string jumpSound;

    [Header("=================================================================")]
    public Sound[] sounds;

    public WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    public void Init()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject soundObject = new GameObject(i + ". 사운드 파일 이름 : " + sounds[i].soundName);
            sounds[i].SetSoundSource(soundObject.AddComponent<AudioSource>());
            soundObject.transform.SetParent(this.transform);
        }
    }

    public void PlaySound(string _soundName, bool canReplay = false)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_soundName == sounds[i].soundName)
            {
                sounds[i].Play(canReplay);
                return;
            }
        }
    }

    public void StopSound(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_soundName == sounds[i].soundName)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    public void SetLoop(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_soundName == sounds[i].soundName)
            {
                sounds[i].SetLoop();
                return;
            }
        }
    }

    public void SetLoopCancle(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_soundName == sounds[i].soundName)
            {
                sounds[i].SetLoopCancle();
                return;
            }
        }
    }

    public void SetVolume(string _soundName, float _volume)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_soundName == sounds[i].soundName)
            {
                sounds[i].SetVolume(_volume);
                return;
            }
        }
    }

    public void FadeOutSound(string _soundName)
    {
        //StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(_soundName));
    }

    IEnumerator FadeOutCoroutine(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            for (float j = 0.5f; j >= 0; j -= 0.001f)
            {
                if (_soundName == sounds[i].soundName)
                {
                    sounds[i].SetVolume(j);
                    yield return waitTime;
                }
            }
        }
    }

    public void FadeInSound(string _soundName)
    {
        //StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(_soundName));
    }

    IEnumerator FadeInCoroutine(string _soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            for (float j = 0; j <= 0.5f; j += 0.001f)
            {
                if (_soundName == sounds[i].soundName)
                {
                    sounds[i].SetVolume(j);
                    yield return waitTime;
                }
            }
        }
    }

    public void StartBGM()
    {
        //StopAllCoroutines();
        StartCoroutine(StartBGMCoroutine());
    }

    IEnumerator StartBGMCoroutine()
    {   
        yield return new WaitForSeconds(1f);
        PlaySound(introBGM);
        FadeInSound(introBGM);
        yield return new WaitForSeconds(20f);
        FadeOutSound(introBGM);
        yield return new WaitForSeconds(2f);
        PlaySound(mainBGM);
        FadeInSound(mainBGM);
    }

    public void Release()
    {
        // PreEnter에서 수행한 작업 되돌려 주거나 등등...
    }
}
