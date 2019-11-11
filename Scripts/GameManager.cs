using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using ChallengeKit.Pattern;
using System;

public class GameManager : Singleton<GameManager>, NM.IManager
{
    public CameraManager cameraManager;
    public InputManager inputManager;
    public ResourceManager resourceManager;
    public EffectManager effectManager;

    public GameObject player;
    //public GameObject soundManager;

    public string walkSound;

    public Vector3 spawnPoint;
    public Transform starPoint;

    [SerializeField]
    public Camera inGameCamera;

    private void Awake()
    {
        effectManager.Init();
        cameraManager.Enter();

        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        starPoint = GameObject.FindGameObjectWithTag("StarPoint").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = Instantiate(Resources.Load("Prefabs/Player"), spawnPoint, Quaternion.identity) as GameObject;
            SendMessage("FollowPlayerTransform", SendMessageOptions.DontRequireReceiver);
            CameraManager.Instance.SetFollow(player);
        }

        //SoundManager.instance.PlayBGM(SoundManager.instance.playBGMSoundName[1]);
        //SoundManager.instance.audioSourceBgm.volume = 0.5f;

        //inputManager.OnFingerSetAction += PlayWalkSound;
    }

    public void ResetPlayer()
    {
        player.transform.position = spawnPoint;
    }

    public void RestartLevel()
    {
        string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        
    }

    public void Release()
    {
        
    }

    //private void PlayWalkSound()
    //{
    //    SoundManager.instance.PlaySE(walkSound);
    //}

}
